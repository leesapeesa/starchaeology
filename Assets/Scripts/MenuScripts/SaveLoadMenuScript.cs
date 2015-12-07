using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveLoadMenuScript : MonoBehaviour {

    [SerializeField] private Text[] slotLabels;
    private ToggleGroup toggles;

    // Use this for initialization
    void Start() {
        toggles = GetComponent<ToggleGroup>();
        RefreshText();
    }

    /// <summary>
    /// Refresh all the save game slot labels to update them to the latest descriptions
    /// </summary>
    public void RefreshText()
    {
        for (int i = 0; i < 3; ++i)
            if (slotLabels[i] != null && PlayerPrefs.HasKey("gameName" + i))
                slotLabels[i].text = PlayerPrefs.GetString("gameName" + i);
    }

    /// <summary>
    /// Returns the index of the currently active toggle, or null if none are active
    /// </summary>
    public int? GetActiveToggle()
    {
        //can't return any valid value if no toggle is selected
        if (!toggles.AnyTogglesOn()) {
            return null;
        }

        //Identify the selected toggle
        foreach (Toggle t in toggles.ActiveToggles()) {
            switch (t.gameObject.name) {
                case "Slot1":
                    return 0;
                case "Slot2":
                    return 1;
                case "Slot3":
                    return 2;
                default:
                    return null;
            }
        }

        //We should never get here, but let's just have this to make the compiler happy
        return null;
    }
}
