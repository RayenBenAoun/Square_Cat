using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class PlayerOutline : MonoBehaviour
{
    [Header("Drawing")]
    public float minPointDistance = 0.05f;
    public float closeThreshold = 0.3f;
    public int minVerticesToClose = 6;
    public float minPerimeterToClose = 0.75f;
    public float lineWidth = 0.08f;

    [Header("Upgrades")]
    public bool spikedWallsUpgrade = false;
    public bool stunWallsUpgrade = false;
    public bool spikeShotUpgrade = false;
    public bool durationUpgrade = false;
    public bool autoCloseShape = false;

    [Header("Resource")]
    public float maxDrawTime = 5f;
    public float regenRate = 1f;
    public Slider resourceBar;

    [Header("Cover & Combat")]
    public float coverLifetime = 3f;   // base lifetime
    public int damageOnClose = 1;
    public GameObject spikePrefab;

    private float currentDrawTime;
    private LineRenderer lr;
    private readonly List<Vector2> points = new List<Vector2>();
    private float pathLength = 0f;
    private bool drawing = false;
    private bool shapeActive = false;
    private float shapeTimer = 0f;
    private GameObject activeCover;
    private EdgeCollider2D storedCoverCollider;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 0;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        currentDrawTime = maxDrawTime;

        if (resourceBar != null)
            resourceBar.maxValue = maxDrawTime;
    }

    void Update()
    {
        // ---------- HANDLE ACTIVE WALL ----------
        if (shapeActive)
        {
            shapeTimer -= Time.deltaTime;

            // auto spike launch right before wall disappears
            if (spikedWallsUpgrade && spikeShotUpgrade && shapeTimer <= 0.15f)
                ShootSpikesOutward();

            // manual shot (press Space again while wall is active)
            if (spikedWallsUpgrade && spikeShotUpgrade &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ShootSpikesOutward();
            }

            if (shapeTimer <= 0f)
            {
                ClearLine();
                shapeActive = false;
            }

            UpdateResourceUI();
            return;
        }

        // ---------- INPUT FOR DRAWING ----------
        bool start = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool hold = Keyboard.current.spaceKey.isPressed;
        bool end = Keyboard.current.spaceKey.wasReleasedThisFrame;

        if (!drawing && start && currentDrawTime > 0f)
            StartDraw();

        if (drawing && hold)
            ContinueDraw();

        if (drawing && end)
            EndDraw();
        else if (!drawing)
            RegenerateResource();

        UpdateResourceUI();
    }

    // ---------- PUBLIC UPGRADE HOOKS ----------

    public void EnableDurationUpgrade() => durationUpgrade = true;
    public void EnableSpikeWalls() => spikedWallsUpgrade = true;
    public void EnableSpikeShot() => spikeShotUpgrade = true;
    public void EnableStunWalls() => stunWallsUpgrade = true;

    // ---------- RESOURCE UI ----------

    void UpdateResourceUI()
    {
        if (resourceBar != null)
            resourceBar.value = currentDrawTime;
    }

    // ---------- DRAWING FLOW ----------

    void StartDraw()
    {
        drawing = true;
        points.Clear();
        lr.positionCount = 0;
        pathLength = 0f;

        Vector2 p = transform.position;
        AddPoint(p);
        AddPoint(p);
    }

    void ContinueDraw()
    {
        currentDrawTime -= Time.deltaTime;
        if (currentDrawTime <= 0f)
        {
            EndDrawingAsCover();
            return;
        }

        Vector2 p = transform.position;

        if (lr.positionCount >= 2)
            lr.SetPosition(lr.positionCount - 1, p);

        if (points.Count == 0 || Vector2.Distance(points[^1], p) >= minPointDistance)
            AddPoint(p);

        if (autoCloseShape && CanAttemptClose())
            CloseAndResolve();
        else if (!autoCloseShape && CanAttemptClose() && IsClosedByDistance())
            CloseAndResolve();
    }

    void EndDraw()
    {
        drawing = false;

        if (autoCloseShape && CanAttemptClose())
            CloseAndResolve();
        else if (CanAttemptClose() && IsClosedByDistance())
            CloseAndResolve();
        else
            EndDrawingAsCover();
    }

    void RegenerateResource()
    {
        if (currentDrawTime < maxDrawTime)
        {
            currentDrawTime += Time.deltaTime * regenRate;
            if (currentDrawTime > maxDrawTime)
                currentDrawTime = maxDrawTime;
        }
    }

    void AddPoint(Vector2 p)
    {
        if (points.Count > 0)
            pathLength += Vector2.Distance(points[^1], p);

        points.Add(p);
        lr.positionCount = points.Count;
        lr.SetPosition(points.Count - 1, p);
    }

    bool CanAttemptClose() => points.Count >= minVerticesToClose && pathLength >= minPerimeterToClose;
    bool IsClosedByDistance() => Vector2.Distance(points[0], points[^1]) <= closeThreshold;

    // ---------- COVER CREATION ----------

    void CloseAndResolve()
    {
        drawing = false;
        Vector2 first = points[0];
        points[^1] = first;
        lr.SetPosition(points.Count - 1, first);

        CreateCoverObject();
        shapeActive = true;

        float lifetime = durationUpgrade ? coverLifetime * 1.5f : coverLifetime;
        shapeTimer = lifetime;
    }

    void EndDrawingAsCover()
    {
        drawing = false;
        CreateCoverObject();
        shapeActive = true;

        float lifetime = durationUpgrade ? coverLifetime * 1.5f : coverLifetime;
        shapeTimer = lifetime;

        ClearLine();
    }

    void CreateCoverObject()
    {
        activeCover = new GameObject("Cover");
        var lrCopy = activeCover.AddComponent<LineRenderer>();

        lrCopy.useWorldSpace = true;
        lrCopy.widthMultiplier = lineWidth;
        lrCopy.positionCount = lr.positionCount;
        lrCopy.SetPositions(GetLinePositions());

        lrCopy.material = new Material(Shader.Find("Sprites/Default"));
        lrCopy.startColor = stunWallsUpgrade ? Color.cyan : Color.white;
        lrCopy.endColor = stunWallsUpgrade ? Color.blue : Color.white;

        storedCoverCollider = activeCover.AddComponent<EdgeCollider2D>();
        storedCoverCollider.edgeRadius = 0.05f;
        storedCoverCollider.SetPoints(points);

        if (spikedWallsUpgrade)
        {
            SpikedWall sw = activeCover.AddComponent<SpikedWall>();
            sw.spikePrefab = spikePrefab;
            sw.SpawnSpikesAlongEdge(points, storedCoverCollider);
        }

        // Cover object itself will be destroyed later;
        // spikes and stun behaviour are handled by their own scripts.
        Destroy(activeCover, durationUpgrade ? coverLifetime * 1.5f : coverLifetime);
    }

    // ---------- SPIKE FIRING ----------

    void ShootSpikesOutward()
    {
        if (!spikeShotUpgrade) return;
        if (activeCover == null) return;

        SpikedWall sw = activeCover.GetComponent<SpikedWall>();
        if (sw == null) return;

        sw.ShootAllSpikes();
    }

    // ---------- HELPERS ----------

    void ClearLine()
    {
        lr.positionCount = 0;
        points.Clear();
        pathLength = 0f;
    }

    private Vector3[] GetLinePositions()
    {
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        return positions;
    }
}
