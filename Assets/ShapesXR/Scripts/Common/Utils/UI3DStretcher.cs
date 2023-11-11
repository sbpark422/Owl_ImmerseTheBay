using System.Collections.Generic;

#if UNITY_EDITOR
using ShapesXr.Import.Core;
#endif

using UnityEngine;

namespace ShapesXr
{
    public enum ColorModificator
    {
        Set,
        Multiply,
        Invert
    }
    
    public enum ColliderType
    {
        None,
        Box,
        Sphere,
        Capsule,
        Mesh
    }

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteAlways]
    public class UI3DStretcher : MonoBehaviour
    {
        #region Fields

        #region SerializedFields
        
        [SerializeField] private Mesh _sourceMesh;
        [SerializeField] private bool _recalculateNormals;
        
        [SerializeField] private Vector3 _pivotPosition = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] private Vector3 _sourceMeshRotation = Vector3.zero;
        [SerializeField] private Vector3 _finalPositionOffset = Vector3.zero;

        [SerializeField] private Vector3 _size = Vector3.one;
        [SerializeField] private bool _customChamfer;
        [SerializeField] private float _chamferScale = 1f;
        [SerializeField] private float _outerMeshScale = 1f;

        [SerializeField] private Vector3 _slicePlus = 0.51f * Vector3.one;
        [SerializeField] private Vector3 _sliceMinus = 0.49f * Vector3.one;
        [SerializeField] private bool _showSliceLine = true;
        
        [SerializeField] private bool _setCustomColor;
        [SerializeField] private ColorModificator _colorModificator = ColorModificator.Set;
        [SerializeField] private Color _color;
        [SerializeField] private bool _setAlpha;
        
        [SerializeField] private float _colliderSize = 1;
        [SerializeField] private ColliderType _colliderType;
        
        #endregion

        #region PrivateFields
        
        private MeshFilter _meshFilter;
        private Mesh _preScaleMesh;
        private Mesh _mesh;
        
        private bool _initialized;

        private Bounds _sourceMeshBounds;
        
        private Bounds _newMeshBounds;

        private Vector3[] _sourceVertices;
        private Vector3[] _sourceNormals;
        private Vector3[] _newVertices;

        private Color[] _sourceColors;
        private List<Color> _colorBuffer;

        private Collider[] _colliders;
        
        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;
        private CapsuleCollider _capsuleCollider;
        private MeshCollider _meshCollider;

        private Vector3[] _gizmoPoints = new Vector3[4];
        
        #endregion
        
        #endregion

        #region Properties
        
        #region MeshProperties
        
        public Mesh SourceMesh
        {
            get => _sourceMesh;
            set
            {
                if (_sourceMesh == value)
                {
                    return;
                }

                _sourceMesh = value;
                
                ResetAll();
            }
        }

        public bool RecalculateNormals
        {
            get => _recalculateNormals;
            set
            {
                if (_recalculateNormals == value)
                {
                    return;
                }

                _recalculateNormals = value;

                ResetAll();
            }
        }
        
        #endregion

        #region TransformProperties
        
        public Vector3 PivotPosition
        {
            get => _pivotPosition;
            set
            {
                if (_pivotPosition == value)
                {
                    return;
                }

                _pivotPosition = value;
                
                ResetAll();
            }
        }

        public Vector3 SourceMeshRotation
        {
            get => _sourceMeshRotation;
            set
            {
                if (_sourceMeshRotation == value)
                {
                    return;
                }

                _sourceMeshRotation = value;
                
                ResetAll();
            }
        }
        
        public Vector3 FinalPositionOffset
        {
            get => _finalPositionOffset;
            set
            {
                if (_finalPositionOffset == value)
                {
                    return;
                }

                _finalPositionOffset = value;
                
                UpdateMeshNoSource();
            }
        }
        
        #endregion

        #region ParametersProperties
        
        public Vector3 Size
        {
            get => _size;
            set
            {
                if (_size == value && _meshFilter && _meshFilter.sharedMesh)
                {
                    return;
                }

                _size = value;

                if (ChamferScale != 0 && !CustomChamfer)
                {
                    GetMeshFromSource();
                }

                UpdateMeshNoSource();
            }
        }
        
        public bool CustomChamfer
        {
            get => _customChamfer;
            set
            {
                if (_customChamfer == value)
                {
                    return;
                }

                _customChamfer = value;
                
                ResetAll();
            }
        }
        
        public float ChamferScale
        {
            get => _chamferScale;
            set
            {
                if (_chamferScale == value)
                {
                    return;
                }

                _chamferScale = value;
                
                ResetAll();
            }
        }
        
        public float OuterMeshScale
        {
            get => _outerMeshScale;
            set
            {
                if (_outerMeshScale == value)
                {
                    return;
                }

                _outerMeshScale = value;
                
                UpdateMeshNoSource();
            }
        }
        
        #endregion

        #region SlicingProperties

        public Vector3 SliceMinus
        {
            get => _sliceMinus;
            set
            {
                if (_sliceMinus == value)
                {
                    return;
                }

                _sliceMinus = value;
                
                ResetAll();
            }
        }        
        
        public Vector3 SlicePlus
        {
            get => _slicePlus;
            set
            {
                if (_slicePlus == value)
                {
                    return;
                }

                _slicePlus = value;
                
                ResetAll();
            }
        }

        public bool ShowSliceLine
        {
            get => _showSliceLine;
            set => _showSliceLine = value;
        }
        
        #endregion

        #region ColorProperties
        
        public bool SetCustomColor
        {
            get => _setCustomColor;
            set
            {
                if (_setCustomColor == value)
                {
                    return;
                }

                _setCustomColor = value;

                ResetColor();
            }
        }

        public ColorModificator ColorModificator
        {
            get => _colorModificator;
            set
            {
                if (_colorModificator == value)
                {
                    return;
                }

                _colorModificator = value;
                
                ResetColor();
            }
        }
        
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value)
                {
                    return;
                }

                _color = value;
                
                ResetColor();
            }
        }
        
        public bool SetAlpha
        {
            get => _setAlpha;
            set
            {
                if (_setAlpha == value)
                {
                    return;
                }

                _setAlpha = value;

                ResetColor();
            }
        }
        
        #endregion
        
        #region CollisionProperties
        
        public ColliderType ColliderType
        {
            get => _colliderType;
            set
            {
                if (_colliderType == value)
                {
                    return;
                }

                _colliderType = value;
                
                ResetCollider();
            }
        }
        
        public float ColliderSize
        {
            get => _colliderSize;
            set
            {
                if (_colliderSize == value)
                {
                    return;
                }

                _colliderSize = value;
                
                UpdateColliderParameters();
            }
        }
        
        #endregion

        #endregion
        
        private void OnEnable()
        {
            if (_initialized)
            {
                return;
            }
            
            ResetAll();
            _initialized = true;
        }
        
        public void ResetAll()
        {
            ResetMesh();
            ResetCollider();
            ResetColor();
        }
        
        public void ResetMesh()
        {
            GetMeshFromSource();
            StretchMesh();
        }

        private void UpdateMeshNoSource()
        {
            StretchMesh();
            UpdateColliderParameters();
            ResetColor();
        }

        private void ResetCollider()
        {
            GetOrAddCollider();
            UpdateColliderParameters();
        }
        
        public void ResetSize()
        {
            if (!SourceMesh)
            {
                return;
            }

            Vector3 meshBounds = SourceMesh.bounds.size;

            ChamferScale = 1;
            Size = meshBounds;
        }

        private void StretchMesh() {
            if (!_mesh)
            {
                GetMeshFromSource();
            }
            
            Vector3 offset = 0.5f * (Size - _newMeshBounds.size);

            Vector3 correctPivot = 0.5f * _newMeshBounds.size - _newMeshBounds.center;

            Vector3 slicePlusThreshold = Vector3.Scale(_newMeshBounds.size, SlicePlus) - correctPivot;
            Vector3 sliceMinusThreshold = Vector3.Scale(_newMeshBounds.size, SliceMinus) - correctPivot;

            Vector3 pivotOffset = Vector3.Scale(-PivotPosition + 0.5f * Vector3.one, Size);

            for (int i = 0; i < _sourceVertices.Length; i++)
            {
                Vector3 vertex = OuterMeshScale * (_sourceVertices[i] - _newMeshBounds.center + pivotOffset) + FinalPositionOffset;
                
                for (int j = 0; j < 3; j++)
                {
                    if (_sourceVertices[i][j] > slicePlusThreshold[j])
                    {
                        vertex[j] += offset[j] * OuterMeshScale;
                    }
                    else if (_sourceVertices[i][j] < sliceMinusThreshold[j])
                    {
                        vertex[j] -= offset[j] * OuterMeshScale;
                    }
                }

                _newVertices[i] = vertex;
            }

            _mesh.vertices = _newVertices;
            
            if (RecalculateNormals)
            {
                _mesh.RecalculateNormals();
            }

            _mesh.RecalculateBounds();
        }

        private void UpdateColliderParameters()
        {
            if (ColliderType == ColliderType.None)
            {
                return;
            }
            
            if (!_mesh)
            {
                ResetMesh();
            }

            if (!_boxCollider && !_sphereCollider && !_capsuleCollider)
            {
                GetOrAddCollider();
            }

            _mesh.RecalculateBounds();

            switch (ColliderType)
            {
                case ColliderType.Box:

                    _boxCollider.center = _mesh.bounds.center;
                    _boxCollider.size = ColliderSize * _mesh.bounds.size;
                    break;

                case ColliderType.Sphere:

                    _sphereCollider.center = _mesh.bounds.center;
                    _sphereCollider.radius = 0.5f * ColliderSize * _mesh.bounds.size.MaxComponent();
                    break;

                case ColliderType.Capsule:

                    Vector3 boundsSize = _mesh.bounds.size;

                    float max = boundsSize.MaxComponent();
                    float min = boundsSize.MinComponent();
                    float mid = Vector3.Dot(boundsSize, Vector3.one) - max - min;

                    for (int i = 0; i < 3; i++)
                    {
                        if (boundsSize[i] != max)
                        {
                            continue;
                        }

                        _capsuleCollider.direction = i;
                        break;
                    }

                    _capsuleCollider.center = _mesh.bounds.center;
                    _capsuleCollider.radius = 0.5f * (mid + max * (ColliderSize - 1));
                    _capsuleCollider.height = max + max * (ColliderSize - 1);
                    break;
                
                case ColliderType.Mesh:
                    
                    _meshCollider.sharedMesh = _mesh;
                    break;
            }
        }

        private void GetOrAddCollider()
        {
            Collider activeCollider = null;

            bool newColliderAdded = false;            
            
            switch (ColliderType)
            {
                case ColliderType.Box:
                {
                    if(!_boxCollider)
                    {
                        _boxCollider = gameObject.GetOrAddComponent<BoxCollider>();
                        newColliderAdded = true;
                    }
                    
                    activeCollider = _boxCollider;
                    
                    break;
                }
                case ColliderType.Sphere:
                {
                    if(!_sphereCollider)
                    {
                        _sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
                        newColliderAdded = true;
                    }

                    activeCollider = _sphereCollider;
                    
                    break;
                }
                case ColliderType.Capsule:
                {
                    if(!_capsuleCollider)
                    {
                        _capsuleCollider = gameObject.GetOrAddComponent<CapsuleCollider>();
                        newColliderAdded = true;
                    }

                    activeCollider = _capsuleCollider;
                    break;
                }
                case ColliderType.Mesh:
                {
                    if(!_meshCollider)
                    {
                        _meshCollider = gameObject.GetOrAddComponent<MeshCollider>();
                        newColliderAdded = true;
                    }

                    activeCollider = _meshCollider;
                    break;
                }
            }
            
            if(newColliderAdded)
            {
                _colliders = GetComponents<Collider>();
            }

            if (_colliders == null)
            {
                return;
            }
            
            foreach (var col in _colliders)
            {
                col.enabled = col == activeCollider;
            }
        }

        private void GetMeshFromSource()
        {
            if(!_meshFilter)
            {
                _meshFilter = GetComponent<MeshFilter>();
            }

            if (!SourceMesh)
            {
                _meshFilter.sharedMesh = null;
                return;
            }

            _mesh = Instantiate(SourceMesh);
            _meshFilter.mesh = _mesh;
            _sourceMeshBounds = _mesh.bounds;
            
            _sourceVertices = _mesh.vertices;
            _sourceNormals = _mesh.normals;
            
            int vertexCount = _sourceVertices.Length;
            int normalsCount = _sourceNormals.Length;
            
            _sourceColors = SourceMesh.colors.Length < vertexCount ? new Color[vertexCount] : SourceMesh.colors;
            _colorBuffer = new List<Color>(vertexCount);
            
            _newVertices = new Vector3[vertexCount];

            Vector3 chamferThreshold = Vector3.Scale(SliceMinus + (Vector3.one - SlicePlus), _sourceMeshBounds.size);
            Vector3 chamferScales = Vector3.Scale(Size, chamferThreshold.ComponentwiseInverse());
            float chamferScalesMin = chamferScales.MinComponent();
            
            if (SourceMeshRotation != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.Euler(SourceMeshRotation);

                for (int i = 0; i < vertexCount; i++)
                {
                    _sourceVertices[i] = newRotation * (_sourceVertices[i] - PivotPosition) + PivotPosition;
                }

                for (int i = 0; i < normalsCount; i++)
                {
                    _sourceNormals[i] = newRotation * _sourceNormals[i];
                }
            }

            if (ChamferScale != 0)
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    float chamferScale = ChamferScale;

                    if (!CustomChamfer)
                    {
                        chamferScale = Mathf.Min(chamferScalesMin, chamferScale);
                        chamferScale = Mathf.Max(chamferScale, 0.005f);
                    }

                    _sourceVertices[i] *= chamferScale;
                }
            }

            _preScaleMesh = _mesh;
            _preScaleMesh.vertices = _sourceVertices;
            _preScaleMesh.normals = _sourceNormals;

            _preScaleMesh.RecalculateBounds();
            _newMeshBounds = _preScaleMesh.bounds;
        }

        private void ResetColor()
        {
            if (!SetCustomColor || !_mesh || _colorBuffer == null)
            {
                ResetMesh();
            }
            
            if (!SetCustomColor)
            {
                return;
            }

            switch (_colorModificator)
            {
                case ColorModificator.Multiply:
                    _mesh.MultiplyVertexColorNonAlloc(_sourceColors, _colorBuffer, _color, SetAlpha);
                    break;
                case ColorModificator.Invert:
                    _mesh.InvertVertexColorNonAlloc(_sourceColors, _colorBuffer, SetAlpha);
                    break;
                default:
                    _mesh.SetVertexColorNonAlloc(_colorBuffer, _color, SetAlpha);
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!SourceMesh || !_showSliceLine || Application.isPlaying)
            {
                return;
            }

            if (!_mesh)
            {
                ResetAll();
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            
            Vector3 pivotOffset = Vector3.Scale(-PivotPosition + 0.5f * Vector3.one, Size);

            Vector3 extents = _newMeshBounds.extents;
            Vector3 width = 0.5f * Size - extents;

            Vector3 pointMin = Vector3.Scale(_newMeshBounds.size, SliceMinus) - extents - width + pivotOffset;
            Vector3 pointMax = Vector3.Scale(_newMeshBounds.size, SlicePlus) - extents + width + pivotOffset;
            
            Vector3 boundsMin = _mesh.bounds.min;
            Vector3 boundsMax = _mesh.bounds.max;
            
            _gizmoPoints[0] = pointMin;
            _gizmoPoints[1] = pointMax.SetX(pointMin.x);
            _gizmoPoints[2] = pointMax.SetY(pointMin.y);
            _gizmoPoints[3] = pointMax.SetZ(pointMin.z);
            
            for(int i = 0; i < 3; i++)
            {
                for (int p = 0; p < 4; p++)
                {
                    Gizmos.DrawLine(_gizmoPoints[p].SetI(i, boundsMin[i]), _gizmoPoints[p].SetI(i, boundsMax[i]));
                }
            }
        }

    }

#if !UNITY_EDITOR
    public static class Ui3DStretcherExtensions
    {

        public static float MaxComponent(this Vector3 vector) => Mathf.Max(Mathf.Max(vector.x, vector.y), vector.z);
        public static float MinComponent(this Vector3 vector) => Mathf.Min(Mathf.Min(vector.x, vector.y), vector.z);
        
        public static Vector3 ComponentwiseInverse(this Vector3 vector) => new Vector3(1.0f / vector.x, 1.0f / vector.y, 1.0f / vector.z);

        public static Vector3 SetX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);
        public static Vector3 SetY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
        public static Vector3 SetZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

        public static Vector3 SetI(this Vector3 vector, int i, float value)
        {
            if (i >= 0 && i <= 2)
            {
                vector[i] = value;
            }
            
            return vector;
        }
        
        public static Color SetAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

        public static T GetOrAddComponent<T>(this GameObject value) where T : Component
        {
            if (!value.TryGetComponent(out T component))
            {
                component = value.AddComponent<T>();
            }

            return component;
        }

        public static void MultiplyVertexColorNonAlloc(this Mesh mesh, Color[] sourceColors, List<Color> colorBuffer, Color color, bool multiplyAlpha = false)
        {
            mesh.GetColors(colorBuffer);

            int vertexCount = mesh.vertexCount;
            int colorsCount = colorBuffer.Count;

            Color colorA1 = color.SetAlpha(1);

            for (int i = 0; i < vertexCount; i++)
            {
                if (i < colorsCount)
                {
                    colorBuffer[i] = (multiplyAlpha ? color : colorA1) * sourceColors[i];
                }
                else
                {
                    colorBuffer[i] = colorA1;
                }
            }

            mesh.SetColors(colorBuffer);
        }

        public static void InvertVertexColorNonAlloc(this Mesh mesh, Color[] sourceColors, List<Color> colorBuffer, bool invertAlpha = false)
        {
            mesh.GetColors(colorBuffer);

            int vertexCount = mesh.vertexCount;
            int colorsCount = colorBuffer.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                Color sourceColor = sourceColors[i];
                Color result = Color.white - sourceColor;

                if (!invertAlpha)
                {
                    result.a = sourceColor.a;
                }

                if (i < colorsCount)
                {
                    colorBuffer[i] = result;
                }
                else
                {
                    result = Color.cyan;
                    colorBuffer.Add(result);
                }
            }

            mesh.SetColors(colorBuffer);
        }

        public static void SetVertexColorNonAlloc(this Mesh mesh, List<Color> colorBuffer, Color color, bool setAlpha = false)
        {
            mesh.GetColors(colorBuffer);

            int vertexCount = mesh.vertexCount;
            int colorsCount = colorBuffer.Count;

            Color colorA1 = color.SetAlpha(1);

            for (int i = 0; i < vertexCount; i++)
            {
                Color result = colorsCount > 0 ? color : colorA1;
                if (colorsCount > 0 && !setAlpha)
                {
                    result.a = colorBuffer[i].a;
                }

                if (i < colorsCount)
                {
                    colorBuffer[i] = result;
                }
                else
                {
                    colorBuffer.Add(result);
                }
            }

            mesh.SetColors(colorBuffer);
        }
    }
    
#endif
}