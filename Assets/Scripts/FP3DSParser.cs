using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FPBaseChunk
{
    int m_uSize = 0;
    public int Size
    {
        get {
            return m_uSize;
        }
        set {
            m_uSize = value;
        }
    }
}
public class FP3DSChunk : FPBaseChunk
{
    public const int CHUNK_SIZE = 2;
}
public class ParserCommonMethods : MonoBehaviour
{
    protected virtual bool ReadChunk( FPBaseChunk _chunk)
    {
        return true;
    }
    protected virtual FPBaseChunk ReadChunk()
    {
        return null;
    }
    protected virtual int GetRemainingSize( )
    {
        return 0;
    }
    protected virtual void SetReadLimit( int pos)
    {

    }
    protected virtual int GetCurrentPos()
    {
        return 0;
    }

    public virtual GameObject ImportMesh(byte[] _bytes, int _size, string _name)
    {
        using (Stream bit_stream = new MemoryStream(_bytes))
        {
            using (BinaryReader stream_reader = new BinaryReader(bit_stream))
            {
                GameObject game_obj = parse(stream_reader, _name);
                return game_obj;
            }
        }
        return null;
    }
    protected virtual int GetChunkHeadSize()
    {
        return 0;
    }
    protected virtual GameObject parse(BinaryReader stream_reader, string _name = "testobj")
    {
        return null;
    }
}

public enum FZ_3DS_CHECK_SUM_CODE{
    // ********************************************************************

    // Prj master chunk
    CHUNK_PRJ = 0xC23D,

    // MDLI master chunk
    CHUNK_MLI = 0x3DAA,

    // Primary main chunk of the .3ds file
    MAIN_CHUNK = 0x4D4D,
        // Basic chunks which can be found everywhere in the file
        M3D_VERSION = 0x0002,
        CHUNK_RGBF = 0x0010,       // float4 R; float4 G; float4 B
        CHUNK_RGBB = 0x0011,       // int1 R; int1 G; int B

        // Linear color values (gamma = 2.2?)
        CHUNK_LINRGBF = 0x0013,    // float4 R; float4 G; float4 B
        CHUNK_LINRGBB = 0x0012,    // int1 R; int1 G; int B

        CHUNK_PERCENTW = 0x0030,       // int2   percentage
        CHUNK_PERCENTF = 0x0031,       // float4  percentage

        // Mesh main chunk
        EDITOR_CHUNK = 0x3D3D,
            // Specifies the background color of the .3ds file
            // This is passed through the material system for
            // viewing purposes.
            CHUNK_BKGCOLOR = 0x1200,

            // Specifies the ambient base color of the scene.
            // This is added to all materials in the file
            CHUNK_AMBCOLOR = 0x2100,

            // Specifies the background image for the whole scene
            // This value is passed through the material system
            // to the viewer
            CHUNK_BIT_MAP = 0x1100,
            CHUNK_BIT_MAP_EXISTS = 0x1101,

            // ********************************************************************
            // Viewport related stuff. Ignored
            CHUNK_DEFAULT_VIEW = 0x3000,
            CHUNK_VIEW_TOP = 0x3010,
            CHUNK_VIEW_BOTTOM = 0x3020,
            CHUNK_VIEW_LEFT = 0x3030,
            CHUNK_VIEW_RIGHT = 0x3040,
            CHUNK_VIEW_FRONT = 0x3050,
            CHUNK_VIEW_BACK = 0x3060,
            CHUNK_VIEW_USER = 0x3070,
            CHUNK_VIEW_CAMERA = 0x3080,

            // Mesh chunks
            // ********************************************************************
            OBJECT_BLOCK = 0x4000,
                TRIANGULAR_MESH = 0x4100,
                    VERTICES_LIST = 0x4110,
                    FACES_DESCRIPTION = 0x4120,
                        FACES_MATERIAL = 0x4130,
                        SMOOTHING_GROUP_LIST = 0x4150,
                    MAPPING_COORDINATES_LIST = 0x4140,
                    LOCAL_COORDINATES_SYSTEM = 0x4160,
                    CHUNK_MESHCOLOR = 0x4165,
                    CHUNK_TXTINFO = 0x4170,
                LIGHT = 0x4600, 
                    SPOTLIGHT = 0x4610,
                CAMERA = 0x4700,
                CHUNK_HIERARCHY = 0x4F00,
            // Specifies the global scaling factor. This is applied
            // to the root node's transformation matrix
            CHUNK_MASTER_SCALE    = 0x0100,

            // ********************************************************************
            // Material chunks
            MATERIAL_BLOCK = 0xAFFF,
                // asciiz containing the name of the material
                MATERIAL_NAME = 0xA000,
                AMBIENT_COLOR = 0xA010, // followed by color chunk
                DIFFUSE_COLOR = 0xA020, // followed by color chunk
                SPECULAR_COLOR = 0xA030, // followed by color chunk
                // Specifies the shininess of the material
                // followed by percentage chunk
                CHUNK_MAT_SHININESS  = 0xA040,
                CHUNK_MAT_SHININESS_PERCENT  = 0xA041 ,

                // Specifies the shading mode to be used
                // followed by a short
                CHUNK_MAT_SHADING  = 0xA100,

                // NOTE: Emissive color (self illumination) seems not
                // to be a color but a single value, type is unknown.
                // Make the parser accept both of them.
                // followed by percentage chunk (?)
                CHUNK_MAT_SELF_ILLUM = 0xA080,

                // Always followed by percentage chunk  (?)
                CHUNK_MAT_SELF_ILPCT = 0xA084,

                // Always followed by percentage chunk
                CHUNK_MAT_TRANSPARENCY = 0xA050,

                // Diffuse texture channel 0
                TEXTURE_MAP_1 = 0xA200,
                // Contains opacity information for each texel
                CHUNK_MAT_OPACMAP = 0xA210,

                // Contains a reflection map to be used to reflect
                // the environment. This is partially supported.
                CHUNK_MAT_REFLMAP = 0xA220,

                // Self Illumination map (emissive colors)
                CHUNK_MAT_SELFIMAP = 0xA33d,

                // Bumpmap. Not specified whether it is a heightmap
                // or a normal map. Assme it is a heightmap since
                // artist normally prefer this format.
                BUMP_MAP = 0xA230,

                // Specular map. Seems to influence the specular color
                CHUNK_MAT_SPECMAP = 0xA204,

                // Holds shininess data.
                CHUNK_MAT_MAT_SHINMAP = 0xA33C,

                // Scaling in U/V direction.
                // (need to gen separate UV coordinate set
                // and do this by hand)
                CHUNK_MAT_MAP_USCALE      = 0xA354,
                CHUNK_MAT_MAP_VSCALE      = 0xA356,

                // Translation in U/V direction.
                // (need to gen separate UV coordinate set
                // and do this by hand)
                CHUNK_MAT_MAP_UOFFSET     = 0xA358,
                CHUNK_MAT_MAP_VOFFSET     = 0xA35a,

                // UV-coordinates rotation around the z-axis
                // Assumed to be in radians.
                CHUNK_MAT_MAP_ANG = 0xA35C,

                // Tiling flags for 3DS files
                CHUNK_MAT_MAP_TILING = 0xa351,                
                // Specifies the file name of a texture
                MAPPING_FILENAME = 0xA300,
                // Specifies whether a materail requires two-sided rendering
                CHUNK_MAT_TWO_SIDE = 0xA081,
        // ********************************************************************

        // Main keyframer chunk. Contains translation/rotation/scaling data    
        KEYFRAMER_CHUNK = 0xB000,
            MESH_INFORMATION_BLOCK = 0xB002,
            SPOT_LIGHT_INFORMATION_BLOCK = 0xB007,
            FRAMES = 0xB008,//Start & End
                OBJECT_NAME = 0xB010,
                DUMMY_OBJ_NAME  = 0xB011,
                OBJECT_PIVOT_POINT = 0xB013,
                POSITION_TRACK = 0xB020,
                ROTATION_TRACK = 0xB021,
                SCALE_TRACK = 0xB022,
                HIERARCHY_POSITION = 0xB030,
                // Keyframes for various other stuff in the file
            // Partially ignored
            CHUNK_AMBIENTKEY    = 0xB001,
            CHUNK_TRACKMORPH    = 0xB026,
            CHUNK_TRACKHIDE     = 0xB029,
            CHUNK_OBJNUMBER     = 0xB030,
            CHUNK_TRACKCAMERA   = 0xB003,
            CHUNK_TRACKFOV      = 0xB023,
            CHUNK_TRACKROLL     = 0xB024,
            CHUNK_TRACKCAMTGT   = 0xB004,
            CHUNK_TRACKLIGHT    = 0xB005,
            CHUNK_TRACKLIGTGT   = 0xB006,

    // light sub-chunks
    CHUNK_DL_OFF = 0x4620,
    CHUNK_DL_OUTER_RANGE = 0x465A,
    CHUNK_DL_INNER_RANGE = 0x4659,
    CHUNK_DL_MULTIPLIER = 0x465B,
    CHUNK_DL_EXCLUDE = 0x4654,
    CHUNK_DL_ATTENUATE = 0x4625,
    CHUNK_DL_SPOTLIGHT = 0x4610,

    // camera sub-chunks
    CHUNK_CAM_RANGES = 0x4720
};

public class FP3DSParser : ParserCommonMethods {


    #region Protected Methods
    protected override int GetChunkHeadSize()
    {
        return 0;
    }
    protected override GameObject parse(BinaryReader stream_reader, string _name)
    {
        Debug.Log("Enter Parse");
        GameObject obj = new GameObject(_name);
        if (stream_reader.BaseStream.Length > GetChunkHeadSize())
        {
            uint chunk_size = 0;
            uint chunk_id = 0;
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            Debug.Log("Before Check Main Chunk");
            
            if ((FZ_3DS_CHECK_SUM_CODE)chunk_id == FZ_3DS_CHECK_SUM_CODE.MAIN_CHUNK)
            {
                parseBody(stream_reader, obj.transform);
            }
        }
        Debug.Log("End Parse");
        return obj;
    }
    void parseBody(BinaryReader stream_reader, Transform _parent = null)
    {
        Debug.Log("Enter parseBody");
        uint chunk_size = 0;
        uint chunk_id = 0;
        while (stream_reader.BaseStream.Position < stream_reader.BaseStream.Length)
        {
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            switch ((FZ_3DS_CHECK_SUM_CODE)chunk_id)
            {
                case FZ_3DS_CHECK_SUM_CODE.M3D_VERSION:
                    Debug.Log("M3D_VERSION");
                    skipBlock(stream_reader, chunk_size);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.EDITOR_CHUNK:
                    parseMesh(stream_reader, chunk_size, _parent);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.KEYFRAMER_CHUNK:
                    Debug.Log("KEYFRAMER_CHUNK");
                    skipBlock(stream_reader, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
            }
        }
        Debug.Log("End parseBody");
    }

    void parseMesh(BinaryReader stream_reader, long _size, Transform _parent = null)
    {
        Debug.Log("Enter parseMesh");
        uint chunk_size = 0;
        uint chunk_id = 0;
        while (stream_reader.BaseStream.Position < _size)
        {
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            switch ((FZ_3DS_CHECK_SUM_CODE)chunk_id)
            {
                case FZ_3DS_CHECK_SUM_CODE.OBJECT_BLOCK:
                    parseObject(stream_reader, chunk_size, _parent);
                    break;
                /*case FZ_3DS_CHECK_SUM_CODE.MATERIAL_BLOCK:
                    break;*/
                default:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
            }
        }
        Debug.Log("End parseMesh");
    }
    void parseObject(BinaryReader stream_reader, long _size, Transform _parent = null)
    {
        Debug.Log("Enter parseObject");
        string obj_name = stream_reader.ReadString();
        GameObject msh_obj = new GameObject(obj_name);

        uint chunk_size = 0;
        uint chunk_id = 0;
        while (stream_reader.BaseStream.Position < _size)
        {
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            switch ((FZ_3DS_CHECK_SUM_CODE)chunk_id)
            {
                case FZ_3DS_CHECK_SUM_CODE.TRIANGULAR_MESH:
                    parseShape(stream_reader, chunk_size, msh_obj);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.LIGHT:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.CAMERA:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
            }
        }
        msh_obj.transform.SetParent(_parent);
        Debug.Log("End parseObject");
    }
    void parseShape(BinaryReader stream_reader, long _size, GameObject _obj)
    {
        Debug.Log("Enter parseShape");
        uint chunk_size = 0;
        uint chunk_id = 0;

        MeshFilter msh_filter = _obj.AddComponent<MeshFilter>();
        _obj.AddComponent<MeshRenderer>();
        Vector3[] new_vertices = null;
        Vector2[] map_coords = null;
        while (stream_reader.BaseStream.Position < _size)
        {
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();

            switch ((FZ_3DS_CHECK_SUM_CODE)chunk_id)
            {
                case FZ_3DS_CHECK_SUM_CODE.VERTICES_LIST:
                    {
                        uint vert_num = stream_reader.ReadUInt16();
                        new_vertices = new Vector3[vert_num];
                        for (int idx = 0; idx < vert_num; idx++)
                        {
                            new_vertices[idx].x = stream_reader.ReadSingle();
                            new_vertices[idx].y = stream_reader.ReadSingle();
                            new_vertices[idx].z = stream_reader.ReadSingle();
                        }
                    }
                    break;
                case FZ_3DS_CHECK_SUM_CODE.FACES_DESCRIPTION:
                    parseFaces(stream_reader, msh_filter, chunk_size);
                    //parseFaces(stream_reader, _mesh_obj, chunk_size,ref new_faces);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.MAPPING_COORDINATES_LIST:
                    {
                        uint coords_num = stream_reader.ReadUInt16();
                        map_coords = new Vector2[coords_num];
                        for (int idx = 0; idx < coords_num; idx++)
                        {
                            map_coords[idx].x = stream_reader.ReadSingle();
                            map_coords[idx].y = stream_reader.ReadSingle();
                        }
                    }
                    break;
                case FZ_3DS_CHECK_SUM_CODE.LOCAL_COORDINATES_SYSTEM:
                    Debug.Log("LOCAL_COORDINATES_SYSTEM");
                    skipBlock(stream_reader, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
            }
        }
        Debug.Log("End parseShape");
    }

    void parseFaces(BinaryReader stream_reader, MeshFilter _mesh_obj, long _size)
    {
        Debug.Log("Enter parseFaces");
        uint face_num = stream_reader.ReadUInt16();
        int[] new_faces = new int[face_num * 3];
        List<string> sub_msh_mtrls = new List<string>();
        uint face_flag = 0;
        for (int i = 0; i < face_num * 3; i++)
        {
            new_faces[i] = stream_reader.ReadUInt16();
            i++;
            new_faces[i] = stream_reader.ReadUInt16();
            i++;
            new_faces[i] = stream_reader.ReadUInt16();

            face_flag = stream_reader.ReadUInt16();
        }

        _mesh_obj.mesh.triangles = new_faces;

        uint chunk_size = 0;
        uint chunk_id = 0;
        long seek_pos = 0;
        while (stream_reader.BaseStream.Position < _size)
        {
            seek_pos = stream_reader.BaseStream.Position;
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            switch ((FZ_3DS_CHECK_SUM_CODE)chunk_id)
            {
                case FZ_3DS_CHECK_SUM_CODE.FACES_MATERIAL:
                    {
                        string material_name = stream_reader.ReadString();

                        if (!m_mapGlobalMaterials.ContainsKey(material_name))
                        {
                            Debug.Log("create material " + material_name);
                            Material new_material = new Material(Shader.Find("Standard"));
                            m_mapGlobalMaterials.Add(material_name, new_material);
                        }

                        int sub_msh_face_count = stream_reader.ReadUInt16();
                        int[] sub_msh_face = new int[sub_msh_face_count * 3];
                        for (int sub_idx = 0; sub_idx < sub_msh_face_count * 3; sub_idx += 3)
                        {
                            uint ref_face_idx = stream_reader.ReadUInt16();
                            sub_msh_face[sub_idx] = new_faces[ref_face_idx * 3];
                            sub_msh_face[sub_idx + 1] = new_faces[ref_face_idx * 3 + 1];
                            sub_msh_face[sub_idx + 2] = new_faces[ref_face_idx * 3 + 2];
                            //sub_msh_face[sub_idx]
                        }
                        _mesh_obj.mesh.SetTriangles(sub_msh_face, sub_msh_mtrls.Count);
                        sub_msh_mtrls.Add(material_name);
                    }
                    stream_reader.BaseStream.Seek(seek_pos + chunk_size, SeekOrigin.Begin);
                    break;
                case FZ_3DS_CHECK_SUM_CODE.SMOOTHING_GROUP_LIST:
                    {
                        Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                        skipBlock(stream_reader, chunk_size);
                    }
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FZ_3DS_CHECK_SUM_CODE)chunk_id).ToString());
                    skipBlock(stream_reader, chunk_size);
                    break;
            }

        }
        string msh_obj_name = _mesh_obj.gameObject.name;
        m_mapMeshMaterials.Add(msh_obj_name, sub_msh_mtrls);
        Debug.Log("End parseFaces");
    }
    void parseMaterial()
    {

    }

    void parseAnimation()
    {

    }
    void skipBlock(BinaryReader _stream, long _size)
    {
        Debug.Log("Skipped Size = " + _size.ToString());
        _stream.BaseStream.Seek(_size - chunk_header_size, SeekOrigin.Current);
    }

    bool FP_3DS_BEGIN_CHUNK()
    {
        if (GetRemainingSize() < FP3DSChunk.CHUNK_SIZE)
            return false;
        FP3DSChunk read_chunk = (FP3DSChunk)ReadChunk();
        int chunk_size = read_chunk.Size - FP3DSChunk.CHUNK_SIZE;
        if (chunk_size <= 0)
            return false;
        SetReadLimit(GetCurrentPos() + chunk_size);
        return true;
    }
    #endregion
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
