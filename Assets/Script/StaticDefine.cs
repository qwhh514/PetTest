using UnityEngine;
using System.Collections;

namespace StaticDefine
{
    public static class FilePath
    {
        //************************************************************************************************
        //Dic----------------------------------------------------------------------------------------------------------------------------------
        //************************************************************************************************
        public const string TAB_PATH = "Tab/";
        public const string TEXTURE_PATH = "Texture/";
        public const string UI_PATH = "UI/";
        public const string PARTICLE_PATH = "Particle/";
        public const string PREFAB_PATH = "Prefab/";


        //************************************************************************************************
        //Jsons----------------------------------------------------------------------------------------------------------------------------------
        //************************************************************************************************
        public const string JSON_PATH_NAME = TAB_PATH;

        public const string JSON_ALLJSON_NAME = "all_json";
        public const string JSON_PET_NAME = "pet";
        public const string JSON_SKILL_NAME = "skill";
		public const string JSON_PARTICLE_NAME = "Particles";

        public const string JSON_ALLJSON_PATH = JSON_PATH_NAME + JSON_ALLJSON_NAME;
		public const string JSON_PET_PATH = JSON_PATH_NAME + JSON_PET_NAME;
		public const string JSON_SKILL_PATH = JSON_PATH_NAME + JSON_SKILL_NAME;
		public const string JSON_PARTICLE_PATH = JSON_PATH_NAME + JSON_PARTICLE_NAME;

    }
    
	public static class CameraSetting
	{
		public const float ROTATEX = 0.0f;
		public const float ROTATEY = 0.0f;
		public const float ROTATEX_SPEED = 0.36f;
		public const float ROTATEY_SPEED = 0.36f;
		
		public const int FOV = 0;
		public const float FOV_SPEED = 0.1f;
		
		public const short MOVE_SPEED = 100;
		
//		public const Vector3 DISTANCE_TARGET = ;
		
		public const float SHAKE_TIME = 0.5f;
		public const float SHAKE_DELTA = 0.07f/30.0f;
		public const float SHAKE_RANGE = 0.05f;
	}

}


