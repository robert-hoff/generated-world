using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour {


  public GameObject generatedWorld;
  public Transform playerTransform;
  public Transform cameraTransform;
  public Material tileMaterialGame;
  public Material tileMaterialEditor;


  const int nrQuads = 64;
  const float quadWidth = 0.5f;
  private readonly float tileWidth = nrQuads * quadWidth;


  private const int MIN_X = -5;
  private const int MIN_Y = -5;
  private const int MAX_X = 4;
  private const int MAX_Y = 4;


  private readonly Dictionary<Vector2, GameObject> tileLookup = new Dictionary<Vector2, GameObject>();



  void Start() {
    ClearWorld();
    for (int i = MIN_X; i <= MAX_X; i++) {
      for (int j = MIN_Y; j <= MAX_Y; j++) {
        GameObject tile = GenerateAndDisplayTile(i, j, tileMaterialGame);
        tileLookup.Add(new Vector2(i, j), tile);
      }
    }
  }


  // Called by the editor only
  public void GenerateTiles() {
    for (int i = MIN_X; i <= MAX_X; i++) {
      for (int j = MIN_Y; j <= MAX_Y; j++) {
        GenerateAndDisplayTile(i, j, tileMaterialEditor);
      }
    }
  }




  private Vector2 playerPosition;
  private const float playerSpeed = 15f;


  void Update() {

    for (int i = MIN_X; i <= MAX_X; i++) {
      for (int j = MIN_Y; j <= MAX_Y; j++) {
        GameObject tile = tileLookup[new Vector2(i, j)];
        tile.SetActive(false);
      }
    }


    // (x,y) is the current game-world tile
    int x = (int) Mathf.Floor(playerTransform.position.x / tileWidth);
    int y = (int) Mathf.Floor(playerTransform.position.z / tileWidth);


    // Set a 5x5 set of tiles around the player as active (visible)
    /*
    for (int i = x - 2; i <= x + 2; i++) {
      for (int j = y - 2; j <= y + 2; j++) {
        setTileActive(i, j);
      }
    }
    */

    // Set all tiles active (using clipping planes only for controlling size of world)
    for (int i = MIN_X; i <= MAX_X; i++) {
      for (int j = MIN_Y; j <= MAX_Y; j++) {
        setTileActive(i, j);
      }
    }




    Vector3 forwardDirection = cameraTransform.forward;
    forwardDirection.y = 0;
    forwardDirection.Normalize();
    Vector3 rightDirection = Quaternion.AngleAxis(90, Vector3.up) * forwardDirection;


    // playerTransform.rotation = forwardDirection.rotation;
    // playerTransform.Rotate
    playerTransform.rotation = Quaternion.LookRotation(forwardDirection);

    if (Input.GetKey(KeyCode.W)) {
      // playerTransform.Translate(forwardDirection * Time.deltaTime * playerSpeed);
      playerTransform.Translate(Vector3.forward * Time.deltaTime * playerSpeed);
    }
    if (Input.GetKey(KeyCode.S)) {
      // playerTransform.Translate(-forwardDirection * Time.deltaTime * playerSpeed);
      playerTransform.Translate(Vector3.back * Time.deltaTime * playerSpeed);
    }
    if (Input.GetKey(KeyCode.D)) {
      // playerTransform.Translate(rightDirection * Time.deltaTime * playerSpeed);
      playerTransform.Translate(Vector3.right * Time.deltaTime * playerSpeed);
    }
    if (Input.GetKey(KeyCode.A)) {
      // playerTransform.Translate(-rightDirection * Time.deltaTime * playerSpeed);
      playerTransform.Translate(Vector3.left * Time.deltaTime * playerSpeed);
    }


  }


  private void setTileActive(int x, int y) {
    if (x < MIN_X || x > MAX_X || y < MIN_Y || y > MAX_Y) return;
    tileLookup[new Vector2(x, y)].SetActive(true);
  }




  public void TestFunction() {
    // GenerateFlatTile(64, 0.5f, -5, 0);
  }




  public void ClearWorld() {

    List<GameObject> tiles = new List<GameObject>();
    foreach (Transform child in generatedWorld.transform) {
      // Deleting these directly within the loop seemed to create some bugs leaving artifacts behind
      // (placing them in a list first then delete seems to work)
      // GameObject.DestroyImmediate(child.gameObject);
      tiles.Add(child.gameObject);
    }

    foreach (GameObject g in tiles) {
      GameObject.DestroyImmediate(g);
    }
  }






  public GameObject GenerateAndDisplayTile(int tileX, int tileY, Material material) {
    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
    tile.transform.parent = generatedWorld.transform;                               // <-- sets the parent of the tile to a generatedWorld empty
    // tile.transform.parent = transform;                                           // <-- sets the parent of the tile to the empty running the script
    Mesh tileMesh = GenerateFlatTile(nrQuads, quadWidth, tileX, tileY);
    tile.GetComponent<MeshFilter>().sharedMesh = tileMesh;                          // <-- sharedMesh used instead of mesh
    tile.GetComponent<MeshRenderer>().sharedMaterial = material;                    // <-- sharedMaterial used instead of material
    // tile.SetActive(false);                                                       // <-- greys out the tile (will still exist in the editor)
    // tile.GetComponent<MeshCollider>().enabled = false;
    tile.GetComponent<MeshCollider>().sharedMesh = GeneratePlaneColliderMesh();
    float xPos = tileX * tileWidth;
    float zPos = tileY * tileWidth;
    tile.transform.position = new Vector3(xPos, 0, zPos);
    return tile;
  }





  public Mesh GeneratePlaneColliderMesh() {
    float quadWidth = 3.2f;             // <-- dimension of the quads in the collider is 3.2x3.2m
    int nrQuads = 10;
    int nrXVertices = nrQuads + 1;
    int nrZVertices = nrQuads + 1;
    float tileWidth = nrQuads * quadWidth;

    Vector3[] vertices = new Vector3[nrXVertices * nrZVertices];

    for (int i = 0; i < nrXVertices; i++) {
      for (int j = 0; j < nrZVertices; j++) {
        vertices[i + j * nrZVertices] = new Vector3(i * quadWidth, 0, j * quadWidth);

      }
    }

    int[] triangles = new int[nrQuads * nrQuads * 6];
    int vCount = 0; // vertex count
    int tCount = 0; // triangle count

    for (int i = 0; i < nrQuads; i++) {
      for (int j = 0; j < nrQuads; j++) {
        triangles[tCount + 0] = vCount + 0;
        triangles[tCount + 1] = vCount + nrQuads + 1;
        triangles[tCount + 2] = vCount + 1;
        triangles[tCount + 3] = vCount + 1;
        triangles[tCount + 4] = vCount + nrQuads + 1;
        triangles[tCount + 5] = vCount + nrQuads + 2;

        vCount += 1;
        tCount += 6;
      }
      vCount += 1;
    }

    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.triangles = triangles;

    // R: do I need to calculate normals for collider meshes?
    mesh.RecalculateNormals();
    return mesh;
  }



  /*
   * Generate a square and flat tile
   * nrQuads - number of faces along one side
   *
   * In my 10x10 world valid coordinates for (posX, posY) are (-5,-5) to (4,4)
   * These should translate to uv offsets that are in the range (0,0 to (9,9)
   * So therefore just add 5.
   *
   */
  public Mesh GenerateFlatTile(int nrQuads, float quadWidth, int tileX, int tileY) {

    int uvX = tileX + 5;
    int uvY = tileY + 5;

    int nrXVertices = nrQuads + 1;
    int nrZVertices = nrQuads + 1;
    float tileWidth = nrQuads * quadWidth;
    Vector3[] vertices = new Vector3[nrXVertices * nrZVertices];
    Vector2[] uvMap = new Vector2[nrXVertices * nrZVertices];

    for (int i = 0; i < nrXVertices; i++) {
      for (int j = 0; j < nrZVertices; j++) {
        vertices[i + j * nrZVertices] = new Vector3(i * quadWidth, 0, j * quadWidth);

        uvMap[i + j * nrZVertices] = new Vector2((float)i / nrQuads / 10 + uvX / 10f, (float)j / nrQuads / 10 + uvY / 10f);

      }
    }


    int[] triangles = new int[nrQuads * nrQuads * 6];
    int vCount = 0; // vertex count
    int tCount = 0; // triangle count


    for (int i = 0; i < nrQuads; i++) {
      for (int j = 0; j < nrQuads; j++) {
        triangles[tCount + 0] = vCount + 0;
        triangles[tCount + 1] = vCount + nrQuads + 1;
        triangles[tCount + 2] = vCount + 1;
        triangles[tCount + 3] = vCount + 1;
        triangles[tCount + 4] = vCount + nrQuads + 1;
        triangles[tCount + 5] = vCount + nrQuads + 2;

        vCount += 1;
        tCount += 6;
      }
      vCount += 1;
    }



    Mesh mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvMap;


    mesh.RecalculateNormals();
    return mesh;
  }




}





