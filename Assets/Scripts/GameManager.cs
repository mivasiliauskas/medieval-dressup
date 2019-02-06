using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private string gameDataFileName = "data.json";

    public static GameData gameData;

    public GameObject image;

    // Start is called before the first frame update
    void Start()
    {
        LoadGameData();
        var canvas = GameObject.Find("Canvas");
        gameData.characterCustomization.parts.ForEach(part =>
        {
            LoadCharacterCustomizationPart(canvas, part);
        });
        Randomize();
    }

    public void Randomize()
    {
        var rand = new System.Random();

        GameManager.gameData.characterCustomization.parts.ForEach(part =>
        {
            var index = rand.Next(part.sprites.Count);
            part.instance.GetComponent<Image>().sprite = part.sprites[index];
        });
    }

    void LoadCharacterCustomizationPart(GameObject parent, Part part)
    {
        // Load sprites by name as pattern
        var files = Directory.GetFiles("Assets/Resources/Sprites/Character", $"{part.name}_*.png");
        part.sprites = files.Select(file => Resources.Load<Sprite>("Sprites/Character/" + Path.GetFileNameWithoutExtension(file))).ToList();

        // Instantiate image
        var newImage = Instantiate(image, parent.transform.position, Quaternion.identity);
        part.instance = newImage;

        newImage.transform.SetParent(parent.transform);
        newImage.GetComponent<RectTransform>().localPosition = new Vector3(0, part.pos.y, 0);

        var scale = part.scale != 0 ? part.scale : 1;
        newImage.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            GameManager.gameData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }
}
