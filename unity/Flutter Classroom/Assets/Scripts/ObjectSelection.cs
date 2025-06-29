using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    public LayerMask customizableLayer; // Assign a layer to your customizable objects
    public CustomizationMenu customizationMenu; // Reference to the CustomizationMenu script

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, customizableLayer))
            {
                // An object on the customizableLayer was clicked
                CustomizableObject customizableObject = hit.collider.GetComponent<CustomizableObject>();
                if (customizableObject != null)
                {
                    customizationMenu.ShowMenu(customizableObject);
                }
            }
            else
            {
                // Clicked outside of a customizable object, hide the menu
                customizationMenu.HideMenu();
            }
        }
    }
}


