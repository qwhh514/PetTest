using UnityEngine;
using System.Collections;

public class Game {  
	public string loadName;  
	public uint actorGuid = 0;
	private static Game instance;  
	public static Game GetInstance()  
	{  
		if (instance == null)  
			instance = new Game();  
		
		return instance;  
	}  
}  
