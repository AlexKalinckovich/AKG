// ================ ModelState.cs ================

using System.Numerics;
using AKG.Core.Model;
using AKG.Render.Constants;

namespace AKG.Render.States;

public class ModelState
{
    private bool _needsRecalculation = true;
    private int _lastVertexCount = 0;
    private readonly ObjModel _model;

    public Vector3 Center { get; private set; }
    public float Scale { get; private set; } = 1.0f;

    public ModelState(ObjModel model)
    {
        _model = model;
    }

    public void CalculateModelAdjustment()
    {
        if (_model.Vertices.Count == 0 || !_needsRecalculation)
        {
            return;
        }

        CalculateBoundingBox();
        
        CalculateCenter();
        
        CalculateScale();
        
        UpdateState();
    }

    private void CalculateBoundingBox()
    {
        Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);

        foreach (Vector4 vertex in _model.Vertices)
        {
            min = Vector3.Min(min, new Vector3(vertex.X, vertex.Y, vertex.Z));
            max = Vector3.Max(max, new Vector3(vertex.X, vertex.Y, vertex.Z));
        }

        StoreBoundingBoxValues(min, max);
    }

    private void StoreBoundingBoxValues(Vector3 min, Vector3 max)
    {
        _boundingBoxMin = min;
        _boundingBoxMax = max;
    }

    private void CalculateCenter()
    {
        Vector3 sum = _boundingBoxMin + _boundingBoxMax;
        Center = sum * RenderConstants.InitialHalfCoordinateSystem;
    }

    private void CalculateScale()
    {
        Vector3 size = _boundingBoxMax - _boundingBoxMin;
        float modelSize = Vector3.Dot(size, size);
        modelSize = MathF.Sqrt(modelSize);

        float calculatedScale = RenderConstants.DefaultNormalizationFactor / modelSize;
        Scale = Math.Clamp(calculatedScale, RenderConstants.MinimumModelScale, RenderConstants.MaximumModelScale);
    }

    private void UpdateState()
    {
        _needsRecalculation = false;
        _lastVertexCount = _model.Vertices.Count;
    }

    public void MarkAsDirty()
    {
        if (_lastVertexCount != _model.Vertices.Count)
        {
            _needsRecalculation = true;
        }
    }

    private Vector3 _boundingBoxMin;
    private Vector3 _boundingBoxMax;
}