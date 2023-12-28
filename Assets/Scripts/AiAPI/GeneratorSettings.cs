using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorSettings_", menuName = "Data/Settings/GenerationSettings", order = 0)]
public class GeneratorSettings : ScriptableObject
{
    public GenerationSettings generationSettings;
}
[System.Serializable]
public class GenerationSettings
{
    [HideInInspector][SerializeField] public string prompt;
    [HideInInspector][SerializeField] public bool use_story = false;
    [HideInInspector][SerializeField] public bool gui_settings = false;
    [SerializeField] public bool use_memory = false;
    [HideInInspector][SerializeField] public bool use_authors_note = false;
    [HideInInspector][SerializeField] public bool use_world_info = false;
    [SerializeField] public int max_context_length = 1314;
    [SerializeField] public int max_length = 80;
    [SerializeField] public float rep_pen = 1.06f;
    [SerializeField] public int rep_pen_range = 1600;
    [SerializeField] public float rep_pen_slope = 0.9f;
    [SerializeField] public float temperature = 0.69f;
    [SerializeField] public float tfs = 1f; //Tail Free Sampling
    [SerializeField] public float top_a = 0.0f; //Top A Sampling
    [SerializeField] public float top_k = 0.0f; // Top K Sampling
    [SerializeField] public float top_p = 0.69f; //Top P Sampling
    [SerializeField] public float typical = 1.0f; //typical sampling
    [HideInInspector][SerializeField] public int[] sampling_order = { 6, 0, 1, 2, 3, 4, 5 };
    [SerializeField] public bool singleline = false;

    [Tooltip("Not used in the sever, but is used to cut off dialogue if the conversation goes past this input limit")][SerializeField] public int max_history_count = 30;

    public GenerationSettings Clone()
    {
        GenerationSettings newSettings = new GenerationSettings();
        newSettings.gui_settings = gui_settings;
        newSettings.singleline = singleline;
        newSettings.use_story = use_story;
        newSettings.use_memory = use_memory;
        newSettings.max_context_length = max_context_length;
        newSettings.max_length = max_length;
        newSettings.rep_pen = rep_pen;
        newSettings.rep_pen_range = rep_pen_range;
        newSettings.rep_pen_slope = rep_pen_slope;
        newSettings.temperature = temperature;
        newSettings.tfs = tfs;
        newSettings.top_a = top_a;
        newSettings.top_k = top_k;
        newSettings.top_p = top_p;
        newSettings.typical = typical;
        newSettings.sampling_order = sampling_order;

        return newSettings;

    }
}
