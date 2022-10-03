using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    //Get components of type T at positionToCheck. returns true if at least one found and the found components are returned in componentAtPositionList
    public static bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;

        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);

        //Loop through all colliders to get an object of type T

        T tComponent = default(T);

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if(tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        componentsAtPositionList = componentList;

        return found;
    }

    //GetComponents of Type T at box with center point and size and angle. Returns true if at least one found and the found components are returned in the list
    public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentList = new List<T>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(point, size, angle);

        //loop through all colliders to get an object of type T
        for (int i = 0; i < collider2DArray.Length; i++)
        {
            T tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentList;
        return found;
    }

    //Returns the array of components of type T at box with center point and size and angle. The numberOfCollidersToTest for is passed as a parameter.
    //Found components are returned in the array
    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size, float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

        T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];

        for(int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if(collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if(tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }
        return componentArray;
    }
}