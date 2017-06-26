/*
---------------------------------------------------------------------------
3D Model Import Module (FPParser)
---------------------------------------------------------------------------

Copyright (c) 2006-2016, Memar Architect team

All Permission is Sean Toussi.
---------------------------------------------------------------------------
*/

/** @file  FP3DSParser.cpp
 *  @brief Implementation of the 3ds Parsing class
 */

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
    protected BinaryReader Cur_Reader = null;
    protected virtual bool ReadChunk( FPBaseChunk _chunk)
    {
        return true;
    }
    protected virtual FPBaseChunk ReadChunk()
    {
        return null;
    }
    protected virtual long GetRemainingSize( )
    {
        if (Cur_Reader == null)
            return 0;
        return Cur_Reader.BaseStream.Length - Cur_Reader.BaseStream.Position;
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
                Cur_Reader = stream_reader;
                GameObject game_obj = parse(_name);
                return game_obj;
            }
        }
        return null;
    }
    protected virtual int GetChunkHeadSize()
    {
        return 0;
    }
    protected virtual GameObject parse(string _name = "testobj")
    {
        return null;
    }

    protected virtual bool BEGIN_BLOCK()
    {
        return false;
    }

    protected virtual bool END_BLOCK()
    {
        return false;
    }

    protected virtual bool ReadBlock()
    {
        if (!BEGIN_BLOCK())
            return false;
        // todo
        if (!END_BLOCK())
            return false;
        return true;
    }

    protected virtual void skipBlock(long _pos, long _size)
    {
        if (Cur_Reader == null)
            return;
        Debug.Log("Skipped Size = " + _size.ToString());
        Cur_Reader.BaseStream.Seek(_pos - GetChunkHeadSize(), SeekOrigin.Begin);
    }

}
// Flynn 3D Model Parser 3DS File Format Chunk Header
public enum FP_3DS_CH{
    // ********************************************************************

    // Prj master chunk
    PRJ_MASTER = 0xC23D,

    // MDLI master chunk
    MDL_MASTER = 0x3DAA,

    // Primary main chunk of the .3ds file
    MAIN_CHUNK = 0x4D4D,
        // Basic chunks which can be found everywhere in the file
        M3D_VERSION = 0x0002,
        COLOR_FORMAT_RGBF = 0x0010,       // float4 R; float4 G; float4 B
        COLOR_FORMAT_RGBB = 0x0011,       // int1 R; int1 G; int B

        // Linear color values (gamma = 2.2?)
        LINEAR_COLOR_RGBF = 0x0013,    // float4 R; float4 G; float4 B
        LINEAR_COLOR_RGBB = 0x0012,    // int1 R; int1 G; int B

        PERCENT_W = 0x0030,       // int2   percentage
        PERCENT_F = 0x0031,       // float4  percentage

        // Mesh main chunk
        EDITOR_CHUNK = 0x3D3D,
            // Specifies the background color of the .3ds file
            // This is passed through the material system for
            // viewing purposes.
            SCENE_BACKGROUND_COLOR = 0x1200,

            // Specifies the ambient base color of the scene.
            // This is added to all materials in the file
            SCENE_AMBINET_COLOR = 0x2100,

            // Specifies the background image for the whole scene
            // This value is passed through the material system
            // to the viewer
            SCENE_BACKGROUND_BITMAP = 0x1100,
            SCENE_BIT_MAP_EXISTS = 0x1101,

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
                    MESH_COLOR = 0x4165,
                    TEXT_INFO = 0x4170,
                LIGHT = 0x4600, 
                    SPOTLIGHT = 0x4610,
                CAMERA = 0x4700,
                HIERACHY = 0x4F00,
            // Specifies the global scaling factor. This is applied
            // to the root node's transformation matrix
            MASTER_SCALE    = 0x0100,

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
                MATERIAL_SHINESS  = 0xA040,
                MATERIAL_SHINESS_PERCENT  = 0xA041 ,

                // Specifies the shading mode to be used
                // followed by a short
                MATERIAL_SHADING  = 0xA100,

                // NOTE: Emissive color (self illumination) seems not
                // to be a color but a single value, type is unknown.
                // Make the parser accept both of them.
                // followed by percentage chunk (?)
                MATERIAL_SELF_ILLUMINATION = 0xA080,

                // Always followed by percentage chunk  (?)
                MATERIAL_SELF_ILPCT = 0xA084,

                // Always followed by percentage chunk
                MATERIAL_TRANSPARENCY = 0xA050,

                // Diffuse texture channel 0
                MATERIAL_DIFFUSE_MAP_0 = 0xA200,
                // Contains opacity information for each texel
                MATERIAL_OPACITY_MAP = 0xA210,

                // Contains a reflection map to be used to reflect
                // the environment. This is partially supported.
                MATERIAL_REFLECTION_MAP = 0xA220,

                // Self Illumination map (emissive colors)
                MATERIAL_SELF_ILLUMINATION_MAP = 0xA33d,

                // Bumpmap. Not specified whether it is a heightmap
                // or a normal map. Assme it is a heightmap since
                // artist normally prefer this format.
                MATERIAL_BUMP_MAP = 0xA230,

                // Specular map. Seems to influence the specular color
                MATERIAL_SPECULAR_MAP = 0xA204,

                // Holds shininess data.
                MATERIAL_SHINESS_MAP = 0xA33C,

                // Scaling in U/V direction.
                // (need to gen separate UV coordinate set
                // and do this by hand)
                MATERIAL_MAP_USCALE      = 0xA354,
                MATERIAL_MAP_VSCALE      = 0xA356,

                // Translation in U/V direction.
                // (need to gen separate UV coordinate set
                // and do this by hand)
                MATERIAL_MAP_UOFFSET     = 0xA358,
                MATErIAL_MAP_VOFFSET     = 0xA35a,

                // UV-coordinates rotation around the z-axis
                // Assumed to be in radians.
                MATERIAL_MAP_ANGLE = 0xA35C,

                // Tiling flags for 3DS files
                MATERIAL_MAP_TILING = 0xa351,                
                // Specifies the file name of a texture
                MATREIAL_MAP_TEXTURE_NAME = 0xA300,
                // Specifies whether a materail requires two-sided rendering
                MATERIAP_MAP_TWO_SIDE = 0xA081,
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
/// <summary>
///  3DS Model Parser
/// </summary>
public class FP3DSParser : ParserCommonMethods {
    
    #region Protected Methods
    // block header's size
    protected override int GetChunkHeadSize()
    {
        return 6;
    }
    protected override bool BEGIN_BLOCK()
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
    protected void CheckLoadedMeshes(GameObject _obj)
    {
        /*

        for (auto & mesh : mScene->mMeshes)
        {
            if (mesh.mFaces.size() > 0 && mesh.mPositions.size() == 0)
            {
                delete mScene;
                throw DeadlyImportError("3DS file contains faces but no vertices: " + pFile);
            }
            CheckIndices(mesh);
            MakeUnique(mesh);
            ComputeNormalsWithSmoothingsGroups<D3DS::Face>(mesh);
        }*/
    }
    protected void ReplaceDefaultMaterial(GameObject _obj)
    {

    }
    protected void ConvertScene(GameObject _obj)
    {

    }
    protected void GenerateNodeGraph(GameObject _obj)
    {

    }
    protected void ApplyMasterScale(GameObject _obj)
    {

    }      
    /// <summary>
    ///  Enter Point, Parsing Start Entrance
    /// </summary>
    /// <param name="stream_reader"> file stream reader </param>
    /// <param name="_name">result object's name</param>
    /// <returns></returns>
    protected override GameObject parse( string _name)
    {
        Debug.Log("Enter Parse");
        GameObject parse_result_obj = new GameObject(_name);
        // check file status
        // the file should have at least once chuncks
        // parse all meshes in file
        
        while (Cur_Reader.BaseStream.Length > GetChunkHeadSize())
        {
            uint chunk_size = 0;
            uint chunk_id = 0;
            long cur_pos = 0;
            chunk_id = Cur_Reader.ReadUInt16();
            chunk_size = Cur_Reader.ReadUInt32();
            cur_pos = Cur_Reader.BaseStream.Position;
            Debug.Log("Before Check Main Chunk");
            
            if ((FP_3DS_CH)chunk_id == FP_3DS_CH.MAIN_CHUNK)
            {
                parseBody(parse_result_obj);
            }
        }

        // Process all meshes in the file. First check whether all
        // face indices haev valid values. The generate our
        // internal verbose representation. Finally compute normal
        // vectors from the smoothing groups we read from the
        // file.
        CheckLoadedMeshes(parse_result_obj);
        

        // Replace all occurrences of the default material with a
        // valid material. Generate it if no material containing
        // DEFAULT in its name has been found in the file
        ReplaceDefaultMaterial(parse_result_obj);

        // Convert the scene from our internal representation to an
        // aiScene object. This involves copying all meshes, lights
        // and cameras to the scene
        ConvertScene(parse_result_obj);

        // Generate the node graph for the scene. This is a little bit
        // tricky since we'll need to split some meshes into submeshes
        GenerateNodeGraph(parse_result_obj);

        // Now apply the master scaling factor to the scene
        ApplyMasterScale(parse_result_obj);
        Debug.Log("End Parse");
        return parse_result_obj;
    }

    void parseBody(GameObject _parent)
    {
        Debug.Log("Enter parseBody");
        uint chunk_size = 0;
        uint chunk_id = 0;
        long cur_pos = 0;
        while (Cur_Reader.BaseStream.Position < Cur_Reader.BaseStream.Length)
        {
            chunk_id = Cur_Reader.ReadUInt16();
            chunk_size = Cur_Reader.ReadUInt32();
            cur_pos = Cur_Reader.BaseStream.Position;
            switch ((FP_3DS_CH)chunk_id)
            {
                case FP_3DS_CH.M3D_VERSION:
                    Debug.Log("M3D_VERSION");
                    skipBlock(cur_pos, chunk_size);
                    break;
                case FP_3DS_CH.EDITOR_CHUNK:
                    parseMesh(cur_pos, chunk_size, _parent);
                    break;
                case FP_3DS_CH.KEYFRAMER_CHUNK:
                    Debug.Log("KEYFRAMER_CHUNK");
                    skipBlock(cur_pos, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
            }
        }
        Debug.Log("End parseBody");
    }

    void parseMesh(long _pos, long _size, GameObject _parent)
    {
        Debug.Log("Enter parseMesh");
        uint chunk_size = 0;
        uint chunk_id = 0;
        long cur_pos = 0;
        while (Cur_Reader.BaseStream.Position < _size)
        {
            chunk_id = Cur_Reader.ReadUInt16();
            chunk_size = Cur_Reader.ReadUInt32();
            cur_pos = Cur_Reader.BaseStream.Position;
            switch ((FP_3DS_CH)chunk_id)
            {
                case FP_3DS_CH.OBJECT_BLOCK:
                    parseObject(cur_pos, chunk_size, _parent);
                    break;
                /*case FZ_3DS_CHECK_SUM_CODE.MATERIAL_BLOCK:
                    break;*/
                default:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
            }
        }
        Debug.Log("End parseMesh");
    }
    void parseObject(long _pos, long _size, GameObject _parent = null)
    {
        Debug.Log("Enter parseObject");
        string obj_name = Cur_Reader.ReadString();
        GameObject msh_obj = new GameObject(obj_name);

        uint chunk_size = 0;
        uint chunk_id = 0;
        long cur_pos = 0;
        while (Cur_Reader.BaseStream.Position < _size)
        {
            chunk_id = Cur_Reader.ReadUInt16();
            chunk_size = Cur_Reader.ReadUInt32();
            cur_pos = Cur_Reader.BaseStream.Position;
            switch ((FP_3DS_CH)chunk_id)
            {
                case FP_3DS_CH.TRIANGULAR_MESH:
                    parseShape(Cur_Reader, chunk_size, msh_obj);
                    break;
                case FP_3DS_CH.LIGHT:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
                case FP_3DS_CH.CAMERA:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
            }
        }
        msh_obj.transform.SetParent(_parent.transform);
        skipBlock(_pos, _size);
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
        long cur_pos = 0;
        while (stream_reader.BaseStream.Position < _size)
        {
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            cur_pos = Cur_Reader.BaseStream.Position;
            switch ((FP_3DS_CH)chunk_id)
            {
                case FP_3DS_CH.VERTICES_LIST:
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
                case FP_3DS_CH.FACES_DESCRIPTION:
                    parseFaces(stream_reader, msh_filter, chunk_size);
                    //parseFaces(stream_reader, _mesh_obj, chunk_size,ref new_faces);
                    break;
                case FP_3DS_CH.MAPPING_COORDINATES_LIST:
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
                case FP_3DS_CH.LOCAL_COORDINATES_SYSTEM:
                    Debug.Log("LOCAL_COORDINATES_SYSTEM");
                    skipBlock(cur_pos, chunk_size);
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
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
        long cur_pos = 0;
        while (stream_reader.BaseStream.Position < _size)
        {
            cur_pos = stream_reader.BaseStream.Position;
            chunk_id = stream_reader.ReadUInt16();
            chunk_size = stream_reader.ReadUInt32();
            switch ((FP_3DS_CH)chunk_id)
            {
                case FP_3DS_CH.FACES_MATERIAL:
                    {
                        string material_name = stream_reader.ReadString();

                       /* if (!m_mapGlobalMaterials.ContainsKey(material_name))
                        {
                            Debug.Log("create material " + material_name);
                            Material new_material = new Material(Shader.Find("Standard"));
                            m_mapGlobalMaterials.Add(material_name, new_material);
                        }*/

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
                    skipBlock(cur_pos, chunk_size);
                    break;
                case FP_3DS_CH.SMOOTHING_GROUP_LIST:
                    {
                        Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                        skipBlock(cur_pos, chunk_size);
                    }
                    break;
                default:
                    Debug.Log("Skip this chunk + " + ((FP_3DS_CH)chunk_id).ToString());
                    skipBlock(cur_pos, chunk_size);
                    break;
            }

        }
        string msh_obj_name = _mesh_obj.gameObject.name;
        //m_mapMeshMaterials.Add(msh_obj_name, sub_msh_mtrls);
        Debug.Log("End parseFaces");
    }
    void parseMaterial()
    {

    }

    void parseAnimation()
    {

    }


    #endregion
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
