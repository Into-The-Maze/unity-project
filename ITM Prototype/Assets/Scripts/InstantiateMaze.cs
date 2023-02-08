using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstantiateMaze : MonoBehaviour {
    public GameObject layer0Wall;
    public GameObject layer0Floor;
    public GameObject layer0Door;

    //public GameObject layer1Wall;
    //public GameObject layer1Floor;
    
    public GameObject layer2Wall;
    public GameObject layer2WaterFloor;
    public GameObject layer2Floor;
    public GameObject layer2IllusionFloor;

    public GameObject player;

    private void Start() {
        player.transform.position = SetPlayerSpawnPos();
    }
    private void Awake() {
        //InstantiateMazeLayer0(InitialiseLabs());
        InstantiateMazeLayer2(initialiseMazeLayer2());
    }

    public void InstantiateMazeLayer0(char[,] maze) {
        for (int y = 0; y < maze.GetLength(0); y++) {
            for (int x = 0; x < maze.GetLength(1); x++) {
                if (maze[y, x] == ' ') {
                    Instantiate(layer0Floor, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                }
                else if (maze[y, x] == '.') {
                    Instantiate(layer0Door, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                }
                else {
                    Instantiate(layer0Wall, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                }
            }
        }
    }
    public void InstantiateMazeLayer2(char[,] maze) {
        for (int y = 0; y < maze.GetLength(0); y++) {
            for (int x = 0; x < maze.GetLength(1); x++) {
                if (maze[y, x] == '#') {
                    Instantiate(layer2Wall, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                }
                else {
                    System.Random r = new();
                    string floorPicker = Convert.ToString(r.Next(0, 10));
                    if ("01234".Contains(floorPicker)) {
                        Instantiate(layer2Floor, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                    }
                    else if ("56789".Contains(floorPicker)) {
                        Instantiate(layer2WaterFloor, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                    }
                    //ADD ILLUSION FLOOR FUNCTIONALITY LATER BECAUSE THIS METHOD MAKES TOO MANY ILLUSION FLOORS AND THEYRE ALL UNAVOIDABLE

                    //else {
                    //    Instantiate(layer2IllusionFloor, new Vector3(3 * x, 3 * y, 0), Quaternion.identity);
                    //}
                }
            }
        }
    }



    public char[,] initialiseMazeLayer2() {
        char[,] maze = GenerateMazeLayer2.makeBinaryTreeMaze();
        return maze;
    }
    public char[,] InitialiseLabs() {
        char[,] maze = GenerateMazeLayer0.CreateArray();
        GenerateMazeLayer0.Room[] roomList = new GenerateMazeLayer0.Room[0];
        GenerateMazeLayer0.PlaceDefaultAndRandomRooms(ref maze, ref roomList);
        
        int currentRoom = GenerateMazeLayer0.SetStartRoom(ref roomList);
        (char, int, int, int) start;
        return GenerateMazeLayer0.Mazeify(ref maze, ref roomList, ref currentRoom, out start);
    }

    public static Vector3 SetPlayerSpawnPos() {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int attempts = 0;
        while (true) {
            if (attempts == 100) {
                Debug.Log("No Floor found within 100 attempts!");
                return new Vector3(0, 0, 0); 
            }
            GameObject spawnPos = allObjects[(int)Math.Truncate((decimal)UnityEngine.Random.Range(0, allObjects.Length-1))];
            if (spawnPos.CompareTag("FLOOR")) {
                return new Vector3(spawnPos.transform.position.x, spawnPos.transform.position.y, 0);
            }
            attempts++;
        }
    }
        

    
}