using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightToggler : MonoBehaviour
{
    public GameObject directionalLight;
    public float dayRotation;
    public float nightRotation;
    public List<GameObject> lights;
    private bool isDay;

    // Start is called before the first frame update
    void Start()
    {
        // start 
        isDay = false;
        ToggleDayNight();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
           ToggleDayNight();
        } 
    }

    private void ToggleDayNight()
    {
        if (isDay)
        {
            ToNight();
            isDay = false;
        }
        else
        {
            ToDay();
            isDay = true;
        }
    }

    private void ToNight()
    {
        foreach (GameObject light in lights)
        {
            light.gameObject.SetActive(true);
        }
        directionalLight.transform.rotation = Quaternion.Euler(nightRotation, 90f, directionalLight.transform.rotation.z);
    }

    private void ToDay()
    {
        foreach(GameObject light in lights)
        {
            light.gameObject.SetActive(false);
        }

        directionalLight.transform.rotation = Quaternion.Euler(dayRotation, 90f, directionalLight.transform.rotation.z);
    }
}
