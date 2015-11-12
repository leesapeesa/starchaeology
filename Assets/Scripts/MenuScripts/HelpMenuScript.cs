using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpMenuScript : MonoBehaviour {

    // Helper class for navigating between the following help pages
    public GameObject[] helpPages;
	
    void OnEnable() {
        // For some reason, if I start the inner text disabled, I can't get the child again???
        OnClick (helpPages [0].GetComponent<Text>());
    }
    // Turn off all the other's info if one of them is clicked.
    public void OnClick(Text clickedPage) {
        foreach (GameObject page in helpPages) {
            Text compareToPage = page.GetComponent<Text>();
            if (compareToPage.text != clickedPage.text) {
                compareToPage.color = new Color(200, 200, 200);
                page.transform.FindChild("Info").gameObject.GetComponent<Text>().enabled = false;
            } else {
                // We want the info page corresponding to the name to be highlighted.
                compareToPage.color = new Color(255, 255, 255);
                page.transform.FindChild("Info").gameObject.GetComponent<Text>().enabled = true;
            }
        }
    }

}
