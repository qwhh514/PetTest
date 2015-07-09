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
    
}


