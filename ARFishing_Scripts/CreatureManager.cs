using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CreatureManager : MonoBehaviour {

    private static int _numCreatures;
    private static ArrayList _allCreatures;
    private static ArrayList _allCreaturesToDelete; // can't change the list all creatures while inside foreach
    private float _time;
    public static float bulbZ;                      // the center of the light bulb in Vuforia space
    [SerializeField] private float _minRadius;      // input for minRadius
    public static float minRadius;                  // the min radius for bugs circling bulb
    [SerializeField] private float _maxRadius;      // input for maxRadius
    public static float maxRadius;                  // the max radius for bugs circling bulb         
    public static float Zbot;
    public static float Ztop;
    public static bool lightState;

    // Use this for initialization
    void Start () {
        _time = 0;
        _numCreatures = 0;
        _allCreatures = new ArrayList();
        _allCreaturesToDelete = new ArrayList();
        bulbZ = 0.12f;
        maxRadius = _maxRadius;
        minRadius = _minRadius;
        Zbot = bulbZ - .3f;
        Ztop = bulbZ + .3f;
        lightState = false; // light starts as off
        Debug.Log("min radius is " + minRadius);
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        // update all current creatures
        foreach (Insect creature in _allCreatures)
        {
            creature.Update(_time);
        }

        if (_allCreaturesToDelete != null)
        {

            // there are creatures to delete
            foreach (Insect creature in _allCreaturesToDelete)
            {
                _allCreatures.RemoveAt(_allCreatures.IndexOf(creature));
                Destroy(creature.myMesh);
                _numCreatures--;
            }
            _allCreaturesToDelete.Clear();
        }

    }

    public static void AddCreature(Insect newCreature)
        {
            _allCreatures.Add(newCreature);
            _numCreatures++;
        }
    public static void RemoveCreature(Insect oldCreature)
        {
            //Debug.Log("adding bug to delete " + oldCreature.myMesh.name);
            _allCreaturesToDelete.Add(oldCreature);
        }

    public static void ChangeLightState(bool light)
    {
        lightState = light;
        // broadcast to all insects that light state has changed
        foreach (Insect creature in _allCreatures)
        {
            creature.ChangeState(lightState);
        }
    }

}
abstract public class Insect 
{
    public GameObject  myMesh;                  // mesh game object to visually represent this insect
    public const float MAX_RADIUS = 10.0f;      // beyond which the bug is considered to have completely left

    abstract public void SpawnSelf(GameObject bugPrefab, TrackableBehaviour parentObj);
    abstract public void Update(float time);
    abstract public void ChangeState(bool lightstate);

}
public class Gnat : Insect
{
    private float _curRadius;
    private float _curRadiusIncrement;
    private float _curAngle;
    private float _AnglePerSecond;
    private float _curZ;
    private float _curZincrement;


    public override void SpawnSelf(GameObject bugPrefab, TrackableBehaviour parentObj)
    {
        //Debug.Log("++++++++++++ spawing gnat mesh");
        Vector3 startPos = new Vector3(0f, 0f, 0f);
        myMesh = GameObject.Instantiate(bugPrefab, startPos, Quaternion.identity) as GameObject;
        myMesh.transform.parent = parentObj.transform;
        _curRadius          = Random.Range(0.2f, 0.3f);
        _curAngle           = Random.Range(120f, 60.0f);            // degrees to place starting bug in front of bulb
        _AnglePerSecond     = Random.Range(330f, 400.0f);           // speed of this bug
        _AnglePerSecond    *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative 
        _curZ               = CreatureManager.bulbZ;                // start bug at middle of bulb
        _curZincrement      = Random.Range(0.001f, 0.005f);         // move bug orbit along z axis
        _curZincrement     *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative
        _curRadiusIncrement = Random.Range(0.02f, 0.04f);
        _curRadiusIncrement *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative


        //Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        //Debug.Log("< _curRadius > " + _curRadius);
        //Debug.Log("<  radius increment > " + _curRadiusIncrement);
        //Debug.Log("< current angle > " + _curAngle);
        //Debug.Log("< _AnglePerSecond > " + _AnglePerSecond);
    }
    public override void Update(float time)
    {

        float x = Mathf.Cos(Mathf.Deg2Rad * _curAngle) * _curRadius;
        float y = Mathf.Sin(Mathf.Deg2Rad * _curAngle) * _curRadius;
        float z = _curZ;

        myMesh.transform.localPosition = new Vector3(x, y, z);

        _curAngle += (_AnglePerSecond * Time.deltaTime * Mathf.Pow((0.25f / _curRadius), 0.4f));

        if (CreatureManager.lightState)
        {
            // light is on
            if (_curRadius > CreatureManager.maxRadius)
            {
                // bug is far away, must be returning to bulb
                _curRadius -= Mathf.Abs(_curRadiusIncrement);
            }
            else
            {
                // circle light changing increment to keep insect near bulb
                _curRadiusIncrement = Helpers.IncrementRanger(_curRadiusIncrement, _curRadius, CreatureManager.minRadius, CreatureManager.maxRadius);
                _curRadius += _curRadiusIncrement;
            }
        } else
        {
            // light is off and bugs are flying away check for bugs to remove
            _curRadius += Mathf.Abs(_curRadiusIncrement);
            if (_curRadius > MAX_RADIUS)
            {
                // delete this bug
                //Debug.Log("DELETING BUG");
                CreatureManager.RemoveCreature(this);
            }
        }
        _curRadius += _curRadiusIncrement;
        //Debug.Log(">>>>>>>> " + _curRadius);

        _curZincrement = Helpers.IncrementRanger(_curZincrement, _curZ, CreatureManager.Zbot, CreatureManager.Ztop);
        _curZ += _curZincrement;
    }
    public override void ChangeState(bool lightstate)
    {
        if (lightstate)
        {
            // initialize values to create an orbit
            _curRadiusIncrement = Random.Range(-0.02f, -0.04f);
            //Debug.Log("LOOK! a light! " + _curRadiusIncrement);
        } else
        {
            // initialize values to fly away
            _curRadiusIncrement = Random.Range(0.02f, 0.04f);
            //Debug.Log("fly away fly away fly away my little doves " + _curRadiusIncrement);
        }
    }
}
public class Moth : Insect
{
    private float _curRadius;
    private float _curRadiusIncrement;
    private float _curAngle;
    private float _AnglePerSecond;
    private float _curZ;
    private float _curZincrement;


    public override void SpawnSelf(GameObject bugPrefab, TrackableBehaviour parentObj)
    {
        //Debug.Log("++++++++++++ spawing insect mesh");
        Vector3 startPos = new Vector3(0f, 0f, 0f);
        myMesh = GameObject.Instantiate(bugPrefab, startPos, Quaternion.identity) as GameObject;
        myMesh.transform.parent = parentObj.transform;
        _curRadius = Random.Range(0.2f, 0.3f);
        _curAngle = Random.Range(120f, 60.0f);            // degrees to place starting bug in front of bulb
        _AnglePerSecond = Random.Range(270f, 330.0f);           // speed of this bug
        _AnglePerSecond *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative NOT WORKING AS EXPECTED!!!! *********************
        _curZ = CreatureManager.bulbZ;                // start bug at middle of bulb
        _curZincrement = Random.Range(0.001f, 0.005f);         // move bug orbit along z axis
        _curZincrement *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative
        _curRadiusIncrement = Random.Range(0.02f, 0.04f);
        _curRadiusIncrement *= Random.value < 0.5f ? 1 : -1;         // set to positive or negative

        //Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        //Debug.Log("< _curRadius > " + _curRadius);
        //Debug.Log("<  radius increment > " + _curRadiusIncrement);
        //Debug.Log("< current angle > " + _curAngle);
        //Debug.Log("< _AnglePerSecond > " + _AnglePerSecond);
    }
    public override void Update(float time)
    {

        float x = Mathf.Cos(Mathf.Deg2Rad * _curAngle) * _curRadius;
        float y = Mathf.Sin(Mathf.Deg2Rad * _curAngle) * _curRadius;

        float z = _curZ;

        myMesh.transform.localPosition = new Vector3(x, y, z);

        _curAngle += (_AnglePerSecond * Time.deltaTime * Mathf.Pow((0.25f / _curRadius), 0.4f));

        if (CreatureManager.lightState)
        {
            // light is on
            if (_curRadius > CreatureManager.maxRadius)
            {
                // bug is far away, must be returning to bulb
                _curRadius -= Mathf.Abs(_curRadiusIncrement);
            }
            else
            {
                // circle light changing increment to keep insect near bulb
                _curRadiusIncrement = Helpers.IncrementRanger(_curRadiusIncrement, _curRadius, CreatureManager.minRadius, CreatureManager.maxRadius);
                _curRadius += _curRadiusIncrement;
            }
        }
        else
        {
            // light is off and bugs are flying away check for bugs to remove
            _curRadius += Mathf.Abs(_curRadiusIncrement);
            if (_curRadius > MAX_RADIUS)
            {
                // delete this bug
                //Debug.Log("DELETING BUG");
                CreatureManager.RemoveCreature(this);
            }
        }
        _curRadius += _curRadiusIncrement;
        //Debug.Log(">>>>>>>> " + _curRadius);

        _curZincrement = Helpers.IncrementRanger(_curZincrement, _curZ, CreatureManager.Zbot, CreatureManager.Ztop);
        _curZ += _curZincrement;
    }
    public override void ChangeState(bool lightstate)
    {
        if (lightstate)
        {
            // initialize values to create an orbit
            _curRadiusIncrement = Random.Range(-0.02f, -0.04f);
            Debug.Log("LOOK! a light! " + _curRadiusIncrement);
        }
        else
        {
            // initialize values to fly away
            _curRadiusIncrement = Random.Range(0.02f, 0.04f);
            //Debug.Log("fly away fly away fly away my little doves " + _curRadiusIncrement);
        }
    }
}