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

    private List<Sprite> allSprites;
    public static GameData gameData;

    public GameObject selectionButtons;
    public GameObject image;


    // Start is called before the first frame update
    void Start()
    {
        allSprites = Resources.LoadAll("Sprites/Character", typeof(Sprite)).OfType<Sprite>().ToList();
        LoadGameData();
        var characterPlaceholder = GameObject.Find("CharacterPlaceholder");
        var menu = GameObject.Find("Menu");
        var target = GameObject.Find("CharacterPlaceholder");
        gameData.characterCustomization.parts.ForEach(part =>
        {
            LoadCharacterCustomizationPart(part, characterPlaceholder);
            RegisterCharacterCustomizationButton(part, menu);
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

    void RegisterCharacterCustomizationButton(Part part, GameObject parent)
    {
        var group = Instantiate(selectionButtons, parent.transform.position, Quaternion.identity);
        group.transform.SetParent(parent.transform);
        group.transform.localScale = new Vector3(1, 1, 1);

        var parts = gameData.characterCustomization.parts;
        var index = parts.IndexOf(part);
        group.transform.localPosition = new Vector3(40, -40 + index * -15, 0);

        // Set title
        var titleObject = group.transform.Find("Title");
        titleObject.GetComponent<Text>().text = part.title;


        // Set button listeners
        group.transform.Find("ButtonPrevious").GetComponent<Button>().onClick
        .AddListener(delegate { UpdatePartImage(part, -1); });
        group.transform.Find("ButtonNext").GetComponent<Button>().onClick
        .AddListener(delegate { UpdatePartImage(part, +1); });
    }

    void UpdatePartImage(Part part, int modifier)
    {
        var partImage = part.instance.GetComponent<Image>();
        var sprite = partImage.sprite;
        var spriteIndex = part.sprites.IndexOf(sprite) + modifier;
        if (spriteIndex < 0)
        {
            spriteIndex = part.sprites.Count - 1;
        }
        else if (spriteIndex == part.sprites.Count)
        {
            spriteIndex = 0;
        }

        partImage.sprite = part.sprites[spriteIndex];
    }

    void LoadCharacterCustomizationPart(Part part, GameObject parent)
    {
        // Load sprites by name as pattern
        part.sprites = allSprites.Where(x => x.texture.name.StartsWith(part.name)).ToList();
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
