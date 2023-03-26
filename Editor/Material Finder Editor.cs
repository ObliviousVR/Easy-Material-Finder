using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/*
 * Code written by Oblivious!
 * https://twitter.com/ObliviousVR
 */

public class MaterialFinderEditor : EditorWindow
{
    public GameObject myGameObject = null; 

    [MenuItem("Tools/Material Finder")]

    //Necessary for EditorWindow to open, don't mess with this.
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MaterialFinderEditor));
    }

    //Sets the GUI on load
    private void OnGUI()
    {
        GUILayout.Label("GameObject", EditorStyles.boldLabel);
        myGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", myGameObject, typeof(GameObject), true);
        
        //If a gameobject has been placed into the object field, we commit to searching it for materials.
        if (GUILayout.Button("Search", GUILayout.ExpandWidth(true)))
        {
            if (myGameObject != null)
            {
                Debug.Log("Searching...");
                SearchForObjects();
            }
            else
            {
                //Warn the user they haven't done anything yet.
                Debug.Log("You haven't selected a gameobject yet!");
            }
        }
    }

    //Recursively checks the selected object and all of it's child objects for materials assigned to material slots.
    private void SearchForObjects()
    {
        //Get all Mesh Renderers attached to the gameobject and it's children.
        Renderer[] FoundRenders = myGameObject.GetComponentsInChildren<Renderer>(true);

        //Find all the SharedMaterials attached to the renderers we found and add them to a HashSet.
        if (FoundRenders.Length > 0)
        {
            HashSet<Material> ChildMaterials = new HashSet<Material>();

            foreach (Renderer renderer in FoundRenders)
            {
                Material[] unionMaterials = new Material[renderer.sharedMaterials.Length];
                unionMaterials = renderer.sharedMaterials;

                if (unionMaterials.Length > 0)
                {
                    foreach (Material mat in unionMaterials)
                    {
                        ChildMaterials.Add(mat);
                    }
                }
            }

            //Pass the Materials we found over to be displayed in the inspector.
            ShowFoundMaterials(ChildMaterials);
        }
        else
        {
            EditorUtility.DisplayDialog("Whoops!", "We couldn't find any materials attached to the gameobject you selected.", "Ok");
        }
    }

    //Displays the found materials in the inspector.
    private void ShowFoundMaterials(HashSet<Material> materials)
    {
        //Clear the previous selection. Might not be necessary?
        Selection.objects = null;

        //Convert the "materials" HashSet to an array.
        Material[] toSelect = new Material[materials.Count]; 
        materials.CopyTo(toSelect);

        //Set the current selection to the "toSelect" array.
        Selection.objects = toSelect;

        EditorUtility.DisplayDialog("Success!", "Found " + materials.Count + " material(s).", "Ok");
    }

}
