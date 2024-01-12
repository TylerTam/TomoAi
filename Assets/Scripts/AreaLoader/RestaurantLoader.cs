using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RestaurantLoader : AreaLoader
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform tomoCharParent;
    [SerializeField] private List<TomoCharPerson> persons = new List<TomoCharPerson>();
    [SerializeField] private List<ConversationSpot> conversationSpots;
    
    private void Start()
    {
        GameTick.Instance.Ticked += LoadCharacters;
    }
    public void ExitScene()
    {
        GameTick.Instance.Ticked -= LoadCharacters;
    }
    
    private void OnDisable()
    {
        ExitScene();
    }
    private void LoadCharacters() {

        List<ConversationSpot> conversationSpotsTemp = new List<ConversationSpot>(conversationSpots);
        foreach(ConversationSpot spots in conversationSpotsTemp)
        {
            spots.ResetSpots();
        }
        List<NpcAreaData> npcs = GameTick.Instance.NpcPlacements.GetAllCharactersInArea(NpcPlacements.AreaType.Restaurant);
        foreach (NpcAreaData npc in npcs)
        {
            TomoCharPerson person = ObjectPooler.NewObject(npcPrefab, Vector3.zero, Quaternion.identity).GetComponent<TomoCharPerson>();
            person.transform.parent = tomoCharParent;
            person.ConstructTomoChar(GameManager.Instance.SaveLoader.GetCharacterByID(npc.npcId));
            persons.Add(person);

            int randomConvoSpot = 0;
            while (true)
            {
                if (conversationSpotsTemp == null || conversationSpotsTemp.Count == 0)
                {
                    Debug.LogError("Ran out of spots!");
                    break;
                }
                randomConvoSpot = Random.Range(0, conversationSpotsTemp.Count);
                if (conversationSpotsTemp[randomConvoSpot].CanAddPerson(person))
                {
                    break;
                }
                else
                {
                    conversationSpotsTemp.RemoveAt(randomConvoSpot);
                }

            }

        }
    }

    public void ExitToMain()
    {
        FindObjectOfType<SceneLoader>().LoadScene(SceneLoader.SceneNames.SceneSelect);
    }

    public override void LoadArea()
    {
        LoadCharacters();
        PlayerController.Instance.ToggleInteractionInput(true);
    }
}
