/* Written by Cameron Williams
 * 
 * This class defines the variables that each type of map event will use.
 * There are many options so that this class will be flexible enough to
 * accomodate all forms of map events. There are options for a collider
 * to activate the event, a particle system, an animation, an audio source,
 * and a map icon. There is also the option for the map event activating
 * a collection of other events (as with an earthquake swarm), a chance
 * for secondary event (as with a rockfall occuring due to a ridge 
 * earthquake or an eruption occuring fromm an earthquake swarm) and
 * some options for damaging the player (as with event plumes and rock falls).
 * 
 * Last updated 8/5/2019.
 */


using UnityEngine;

public class MapEvent : MonoBehaviour
{
    [Header("Initial Event Options")]
    [SerializeField] public Collider _collider; // all map events have a collider to set off the event
    [SerializeField] public ParticleSystem _particleSystem; // optional particle system
    [SerializeField] public Animation _animation; // optional animation
    [SerializeField] public AudioSource _audioSource; // optional audio source
    [SerializeField] public GameObject _mapIcon; // optional mapIcon to show on 2D Map
    

    [Header("Secondary Chance Event Options")]
    [SerializeField] public bool has_Chance_Event;
    [SerializeField] public GameObject chanceEvent;
    [SerializeField] private int maxChanceRoll = 6;
    [SerializeField] private int chanceRoll;
    [SerializeField] private int chanceActivationThreshold = 4;

    [Header("Event Chain Settings")]
    [SerializeField] public bool event_triggers_event_chain; // enabled if the event can trigger other events
    [SerializeField] public GameObject chainEvent; // event that gets added to _chainEvents if event_triggers_event_chain is true
    [SerializeField] public GameObject[] _chainEvents; // array of events to trigger; e.g. earthquake swarm can trigger other earthquakes
    [SerializeField] private int min_EventsInChain = 3;
    [SerializeField] private int max_EventsInChain = 10;

    [Header("Damage to Player")]
    [SerializeField] public bool eventDamagesPlayer = false; // for events that cause damage; e.g. rockfalls
    [SerializeField] public bool continuousEventDamage = false; // for events that cause damage continuously; e.g. eruptions
    [SerializeField] public float damageTimer = 0.0f; // initial timer for damage ticks
    [SerializeField] public float damageTimerThreshold = 2.0f; // seconds between damage ticks for continuously damaging events
    [SerializeField] public int _eventDamage = 5;

    [Header("Event Plume Variables")]
    [SerializeField] public bool eventPlume = false; // bool for EventPlume only; used to modify BHVs in area around Event Plum gameObject
    [SerializeField] public LayerMask bHVLayermask; // layermask for event plume to look for nearby BHVs
    [SerializeField] public int eventPlumeRadius = 5; // Area around event plume in which BHVs output will be increased
    [SerializeField] public int eventPlumeSupplyMultiplier = 2;

    private void OnAwake()
    {
        if (event_triggers_event_chain)
        {
            _chainEvents = new GameObject[Random.Range(min_EventsInChain, max_EventsInChain)];
            for (int i = 0; i < _chainEvents.Length; i++)
            {
                _chainEvents[i] = chainEvent;
                float eventRadius = GetComponent<SphereCollider>().radius;
                float x_position = Random.Range(-eventRadius, eventRadius);
                float z_position = Random.Range(-eventRadius, eventRadius);
                // if I need to add better y coordinates later
                // Vector3 heightSampleCoordinates = new Vector3(x_position, 0, z_position);
                // float y_position = terrain_below.SampleHeight(heightSampleCoordinates);

                _chainEvents[i].transform.position = new Vector3(x_position, 0, z_position);
                _chainEvents[i].SendMessage("ActivateMapEvent");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ActivateMapEvent();
            if (has_Chance_Event)
            {
                chanceRoll = Random.Range(0, maxChanceRoll);
                if (chanceRoll >= chanceActivationThreshold)
                {
                    chanceEvent.SendMessage("ActivateMapEvent");
                }
            }
            if (eventDamagesPlayer)
            {
                MainManager.Player.DamageHull(_eventDamage);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && continuousEventDamage)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageTimerThreshold)
            {
                MainManager.Player.DamageHull(_eventDamage);
                damageTimer = 0.0f; // reset timer
            }
        }
    }

    public void ActivateMapEvent() // Triggered by the collider or a parent event
    {
        if (eventPlume) // event plumes increase output of nearby BHVs
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, eventPlumeRadius, bHVLayermask);
            int i = 0;
            while (i < hitColliders.Length) 
            {
                if (hitColliders[i].GetComponent<BasaltHostedVent>() != null) // if object is a BHV
                {
                    hitColliders[i].GetComponent<BasaltHostedVent>().EventPlume(eventPlumeSupplyMultiplier, _eventDamage);
                }
            }
        }

        if (_chainEvents != null)
        {
            for (int i = 0; i < _chainEvents.Length; i++)
            {
                _chainEvents[i].SendMessage("ActivateMapEvent");
            }
        }

        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }

        if (_animation != null)
        {
            _animation.Play();
        }

        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        if (_mapIcon != null)
        {
            _mapIcon.SetActive(true);
        }
    }
}
