using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DungeonGeneratorClasses;
using System.IO;
using System.Text;
using System;


public class Main : MonoBehaviour
{
	//RoomType Definition
	//#####################################################################################################
	void initializeRoomTypes ()
	{
		bool[] emptyRoom = new bool[]{ true, true, true, true };
		roomTypes.Add (new Room ("empty", new Color (0, 0, 0), 100.0, ' '));
		roomTypes.Add (new Room ("entrance", new Color (255, 0, 255), 100.0, 'x'));
		roomTypes.Add (new Room ("corridor", new Color (0, 200, 255), 100.0, 'o'));

		roomTypes.Add (new Room ("room1", new Color (0, 255, 0), 100.0, '◙'));
		roomTypes.Add (new Room ("room2", new Color (0, 200, 0), 100.0, '◙'));
		roomTypes.Add (new Room ("room3", new Color (0, 150, 0), 100.0, '◙'));
		roomTypes.Add (new Room ("room4", new Color (0, 100, 0), 100.0, '◙'));
		roomTypes.Add (new Room ("room5", new Color (0, 50, 0), 100.0, '◙'));

		roomTypes.Add (new Room ("boss", new Color (200, 100, 5), 'b'));
	}
	//#####################################################################################################

	// Variables
	//#####################################################################################################

	#region Variables

	const int xDimension = 40, yDimension = 40;

	RoomTile[,] roomTiles;

	RoomTile actTile;

	List<Room> roomTypes = new List<Room> ();

	List<roomDimension> createdRooms = new List<roomDimension> ();

	int[] nextWay;

	int corridorNr = 0, roomNr = 0, mapNr = 0;

	bool alwaysTrue = true;

	OpMode operationMode = OpMode.LoadMap;

	public string openMapFileNr;

	#endregion

	//#####################################################################################################

	// Initialization
	//#####################################################################################################
	void Start ()
	{

		transform.position = new Vector3 (xDimension / 8.0f, yDimension / 8.0f, -(xDimension + xDimension));
		roomTiles = new RoomTile[xDimension, yDimension];
		initializeRoomTypes ();
		initializeRooms ();

		switch (operationMode) {

		case OpMode.ClickOnly:
		case OpMode.AutomaticMode:
			
			nextWay = new int[3];
			nextWay [0] = 10 + UnityEngine.Random.Range (0, xDimension / 2);
			nextWay [1] = 10 + UnityEngine.Random.Range (0, yDimension / 2);
			setRoomType ("entrance", nextWay [0], nextWay [1]);
			//print ("x: " + nextWay [0] + " y: " + nextWay [1] + " emptyness: " + roomTiles [nextWay [0], nextWay [1]].Emptyness);

			break;

		case OpMode.LoadMap:

			//TODO
			break;
		default:
			break;
		}


	}
	//#####################################################################################################

	// Update is called once per frame
	//#####################################################################################################
	void Update ()
	{
		switch (operationMode) {
	
		//################################################################################################
		case OpMode.ClickOnly:

			nextWay = simplePath (); //SIMPLE MAP GENERATION
			if (corridorNr > 5 + UnityEngine.Random.Range (3, 5)) {
				nextWay = simpleRoom ();
				corridorNr = 0;
			}
			corridorNr++;

			if (roomNr >= 5)
				roomNr = 0;

			if (Input.GetKeyDown (KeyCode.S)) //MAPSCORE
				print ("Map Score: " + getMapScore ());

			if (Input.GetMouseButtonDown (1)) //SAVE
				saveMap ("Map", ".txt");

			if (Input.GetMouseButtonDown (0)) {//RESTART
				restart ();
			}
				
			//setRoom (createdRooms [createdRooms.Count - 1], "boss");
			break;
		//################################################################################################
		case OpMode.AutomaticMode:

			restart ();

			while (alwaysTrue) {	

				nextWay = simplePath (); //SIMPLE MAP GENERATION
				if (corridorNr > 5 + UnityEngine.Random.Range (3, 5)) {
					nextWay = simpleRoom ();
					corridorNr = 0;
				}
				corridorNr++;

				if (roomNr >= 5)
					roomNr = 0;

				if (alwaysTrue == false) {
					int actMapScore = (int)getMapScore ();

					mapNr++;

					if (actMapScore > 55000) {
						saveMap ("Map_" + mapNr.ToString (), ".txt");
						print ("Gefunden! Map Score: " + actMapScore);
					}
					print ("Map NR: " + mapNr + " MapScore: " + actMapScore);
				}
			}

			alwaysTrue = true;

			break;

		//################################################################################################
		case OpMode.LoadMap:
			if (Input.GetMouseButtonDown (0)) {
				restart ();
				loadMap ("Map_" + openMapFileNr, ".txt");
			}
			break;
		default:
			break;
		}

	}
	//#####################################################################################################

	void restart ()
	{
		roomNr = 0;
		corridorNr = 0;
		destroyAll ();
		Start ();
	}

	double getMapScore ()
	{
		double totalScore = 0;
		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				totalScore += 100.0 - roomTiles [x, y].Emptyness;
			}
		}
		return totalScore;	
	}

	void initializeRooms ()
	{
		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				actTile = new RoomTile (GameObject.CreatePrimitive (PrimitiveType.Cube));
				actTile.TileObject.transform.position = new Vector3 (0.125f + x / 4.0f, 0.125f + y / 4.0f, 0);
				actTile.TileObject.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);
				roomTiles [x, y] = actTile;

			}
		}
	}

	void setRoomType (string roomType, int xPos, int yPos)
	{
		
		RoomTile roomToChange = roomTiles [xPos, yPos];

		Room roomTypeFound = roomTypes.Find (x => x.Type == roomType);

		overwriteRoom (roomTypeFound, roomToChange);

		setRoomColor (roomToChange);

	}

	void destroyAll ()
	{
		
		foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
			if (o.tag != "MainCamera")
				Destroy (o);
	}

	void overwriteRoom (Room roomTypeFound, RoomTile roomToChange)
	{
		roomToChange.Type = roomTypeFound.Type;
		roomToChange.Color = roomTypeFound.Color;
		roomToChange.Symbol = roomTypeFound.Symbol;
	}

	void setRoomColor (RoomTile roomToChange)
	{
		Material newMaterial = new Material (Shader.Find ("Standard"));
		newMaterial.color = roomToChange.Color;

		roomToChange.TileObject.GetComponent<MeshRenderer> ().material = newMaterial;
	}

	void setRoom (roomDimension rDim, string roomType)
	{
		for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
			for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
				setRoomType (roomType, x, y);
			}
		}
	}

	int[] simpleRoom ()
	{
		roomDimension rDim = spaceforRoom (UnityEngine.Random.Range (3, 7), UnityEngine.Random.Range (3, 7)); //ATM nur gerade

		int[] roomExit = new int[3];

		if (rDim.RoomPossible == true) {
			roomNr++;
			setRoom (rDim, "room" + roomNr.ToString ());

			createdRooms.Add (rDim);

			calculateEmptyness ();

			roomExit [0] = rDim.XExit;
			roomExit [1] = rDim.YExit;
			roomExit [2] = nextWay [2];

			//print ("x: " + roomExit [0] + " y: " + roomExit [1] + " emptyness: " + roomTiles [roomExit [0], roomExit [1]].Emptyness);

			return roomExit;
		} else {
			roomExit = nextWay;
			return roomExit;			
		}

	}

	roomDimension spaceforRoom (int xRoomSize, int yRoomSize)
	{
		roomDimension rDim = new roomDimension ();

		int xStart = nextWay [0], yStart = nextWay [1], direction = nextWay [2];

		switch (direction) {
		case 0:
			rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
			rDim.YRoomCornerMin = yStart + 1;
			rDim.YRoomCornerMax = yStart + yRoomSize;
			rDim.XExit = xStart;
			rDim.YExit = rDim.YRoomCornerMax;
			break;
		case 1:
			rDim.XRoomCornerMin = xStart + 1;
			rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart + xRoomSize;
			rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
			rDim.XExit = rDim.XRoomCornerMax;
			rDim.YExit = yStart;
			break;
		case 2:
			rDim.YRoomCornerMin = yStart - yRoomSize;
			rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
			rDim.YRoomCornerMax = yStart - 1;
			rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
			rDim.XExit = xStart;
			rDim.YExit = rDim.YRoomCornerMin;
			break;
		case 3:
			rDim.XRoomCornerMin = xStart - xRoomSize;			
			rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart - 1;
			rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
			rDim.XExit = rDim.XRoomCornerMin;
			rDim.YExit = yStart;
			break;
		default:
			break;
		}
		if (rDim.XRoomCornerMax >= xDimension || rDim.YRoomCornerMax >= yDimension || rDim.XRoomCornerMin < 0 || rDim.YRoomCornerMin < 0) {
			rDim.RoomPossible = false;
			return rDim;
		}
		if (rDim.XExit >= xDimension || rDim.YExit >= yDimension || rDim.XExit < 0 || rDim.YExit < 0) {
			rDim.RoomPossible = false;
			print ("Exit corridor nicht moeglich!");
			return rDim;
		}
		for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
			for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
				if (roomTiles [x, y].Type != "empty")
					rDim.RoomPossible = false;
			}
		}

		return rDim;
	}

	int[] simplePath ()
	{
		int xStart = nextWay [0], yStart = nextWay [1], nextDirection = chooseWay (xStart, yStart);

		switch (nextDirection) {
		case 0:
			yStart++;
			break;
		case 1:
			xStart++;
			break;
		case 2:
			yStart--;
			break;
		case 3:
			xStart--;
			break;
		case 5:
			//Nothing...
			break;
		default:
			
			break;
		}

		if (checkIfPathFree (xStart, yStart) == false) {
			alwaysTrue = false;
			return nextWay;
		}
		int[] nextRoom = new int[3];
		nextRoom [0] = xStart;
		nextRoom [1] = yStart;
		nextRoom [2] = nextDirection;

		setRoomType ("corridor", nextRoom [0], nextRoom [1]);

		calculateEmptyness ();

		//print ("x: " + nextRoom [0] + " y: " + nextRoom [1] + " emptyness: " + roomTiles [nextRoom [0], nextRoom [1]].Emptyness);

		return nextRoom;
	}

	bool checkIfPathFree (int xCheck, int yCheck)
	{
		if (xCheck >= xDimension || yCheck >= yDimension || xCheck < 0 || yCheck < 0)
			return false;
		if (roomTiles [xCheck, yCheck].Type == "empty") {
			return true;
		} else {
			return false;
		}
	}

	int chooseWay (int xCheck, int yCheck)
	{
		List<int> possibleWays = new List<int> ();
		double bestEmptyness = 0;
		int bestDirection = 1000;
		for (int x = 0; x < 4; x++) {
			switch (x) {
			case 0:
				if (checkIfGoodPath (xCheck, yCheck + 1, x)) {
					if (roomTiles [xCheck, yCheck + 1].Emptyness > bestEmptyness) {
						bestEmptyness = roomTiles [xCheck, yCheck + 1].Emptyness;
						bestDirection = x;
					}
					possibleWays.Add (x);
				}
					

				break;
			case 1:
				
				if (checkIfGoodPath (xCheck + 1, yCheck, x)) {
					if (roomTiles [xCheck + 1, yCheck].Emptyness > bestEmptyness) {
						bestEmptyness = roomTiles [xCheck + 1, yCheck].Emptyness;
						bestDirection = x;
					}
					possibleWays.Add (x);
				}

				break;
			case 2:
				
				if (checkIfGoodPath (xCheck, yCheck - 1, x)) {
					if (roomTiles [xCheck, yCheck - 1].Emptyness > bestEmptyness) {
						bestEmptyness = roomTiles [xCheck, yCheck - 1].Emptyness;
						bestDirection = x;
					}
					possibleWays.Add (x);
				}
				
				break;
			case 3:
				if (checkIfGoodPath (xCheck - 1, yCheck, x)) {
					if (roomTiles [xCheck - 1, yCheck].Emptyness > bestEmptyness) {
						bestEmptyness = roomTiles [xCheck - 1, yCheck].Emptyness;
						bestDirection = x;
					}
					possibleWays.Add (x);
				}

				break;
			default:
				break;
			}
		}
		if (possibleWays.Count < 1)
			return 5;
		if (UnityEngine.Random.Range (0, 8) == 4)
			return possibleWays [UnityEngine.Random.Range (0, possibleWays.Count)];
		else
			return (int)bestDirection;
	}

	bool checkIfGoodPath (int xCheck, int yCheck, int direction)
	{
		switch (direction) {
		case 0:
			if (checkIfPathFree (xCheck, yCheck + 1) && checkIfPathFree (xCheck + 1, yCheck) &&
			    checkIfPathFree (xCheck - 1, yCheck) && checkIfPathFree (xCheck, yCheck))
				return true;
			break;
		case 1:
			if (checkIfPathFree (xCheck + 1, yCheck) && checkIfPathFree (xCheck, yCheck + 1) &&
			    checkIfPathFree (xCheck, yCheck - 1) && checkIfPathFree (xCheck, yCheck))
				return true;
			break;
		case 2:
			if (checkIfPathFree (xCheck, yCheck - 1) && checkIfPathFree (xCheck + 1, yCheck) &&
			    checkIfPathFree (xCheck - 1, yCheck) && checkIfPathFree (xCheck, yCheck))
				return true;
			break;
		case 3:
			if (checkIfPathFree (xCheck - 1, yCheck) && checkIfPathFree (xCheck, yCheck + 1) &&
			    checkIfPathFree (xCheck, yCheck - 1) && checkIfPathFree (xCheck, yCheck))
				return true;
			break;
		default:
			return false;
		}
		return false;
	}

	void calculateEmptyness ()
	{
		double actEmptyness = 0.0;
		int relevantRooms = 26; //1: 8; 2: 26; 3: 50
		int range = 2;

		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				actEmptyness = 0.0; 	
				for (int xLocal = x - range; xLocal <= x + range; xLocal++) {
					for (int yLocal = y - range; yLocal <= y + range; yLocal++) {
						if (xLocal < 0 || xLocal >= xDimension || yLocal < 0 || yLocal >= yDimension || (xLocal == x && yLocal == y)) {
							actEmptyness = actEmptyness + 0;
						} else {
							if (roomTiles [xLocal, yLocal].Type == "empty")
								actEmptyness += 100.0 / relevantRooms;
						}
					}
				}
				roomTiles [x, y].Emptyness = actEmptyness;

			}
		}
	}

	void loadMap (string fileName, string fileEnding)
	{
		string path = @"C:\Users\Oliver\Desktop\" + fileName + fileEnding;

		string readText = File.ReadAllText (path);

		int s = 0;
		for (int y = yDimension - 1; y >= 0; y--) {
			for (int x = 0; x < xDimension; x++) {
				
				Room roomTypeFound = roomTypes.Find (z => z.Symbol == readText [s]);
				setRoomType (roomTypeFound.Type, x, y);
				s++;
			}
			s += 2;
		}
	}

	void saveMap (string fileName, string fileEnding)
	{
		string path = @"C:\Users\Oliver\Desktop\" + fileName + fileEnding;
		//if (!File.Exists (path)) {
		string createText = Environment.NewLine;
		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				createText += roomTiles [x, y].Symbol;
			}
			createText += Environment.NewLine;
		}
		File.WriteAllText (path, createText);
		print ("Printed");
	}





}
	



