using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Compass: MonoBehaviour
{
	float pixelsNorthToNorth = 2048f;
	public RectTransform compassRT;
	public Image arrow;
	public float maxDetectionRadius;
	public float blipFadeTime = 2f;
	public Transform playerTransform;
	public LayerMask detectionLayer;
	[SerializeField] public Color activeBlip = new Color(1, 0, 0, 1);
	[SerializeField] public Color inactiveBlip = new Color(1, 0, 0, 0);
    [SerializeField] private AudioSource detectionSound = null;

	void Start()
	{
		//DetectEnemies();
		InvokeRepeating("DetectEnemies", 1f, 3f);
	}

	void Update() {
		Vector3 perp = Vector3.Cross(Vector3.forward, playerTransform.forward).normalized;
		float dir = -Vector3.Dot(perp, Vector3.up);
		float x = Vector3.Angle(playerTransform.forward, Vector3.forward) * Mathf.Sign(dir) * pixelsNorthToNorth / 360f;
		compassRT.anchoredPosition = new Vector2(x, 0.0f);
	}

    private bool newContactsOnSonar; // used to determine if sonar ping sound should go off; sonar sound should only play when new enemies are in sonar range
	public void DetectEnemies()
	{
		Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, maxDetectionRadius, detectionLayer);
        if (newContactsOnSonar && !detectionSound.isPlaying)
        {
            detectionSound.Play();
        }
        if (hitColliders.Length == 0 || hitColliders == null)
        {
            newContactsOnSonar = true;
        }
        else
        {
            newContactsOnSonar = false;
        }
		for (int i = 0; i < hitColliders.Length; i++)
		{
			float distanceFromPlayer = Vector3.Distance(playerTransform.position, hitColliders[i].gameObject.transform.position);
			Vector3 direction = hitColliders[i].gameObject.transform.position - playerTransform.position;
			float angleFromPlayer = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
			// find distance and angle of each enemy from the player's position
			StartCoroutine(RadarBlipLerp(distanceFromPlayer, angleFromPlayer, arrow, activeBlip, inactiveBlip, blipFadeTime));
			// use coroutine to instantiate new image on compass for each enemy, display after delay based on distance
		}
	}

	public IEnumerator RadarBlipLerp(float distance, float angle, Image imageObject, Color startColor, Color endColor, float timeActive)
	{
		yield return new WaitForSeconds(distance/ (10 * maxDetectionRadius));
		Image indicatorArrow = Instantiate(imageObject, this.transform);
		indicatorArrow.rectTransform.anchoredPosition = new Vector2(angle / 360 * pixelsNorthToNorth, 0f);
		
		float i = 0.0f;
		float rate = (1.0f / timeActive);
		startColor.a = Mathf.Clamp((maxDetectionRadius - distance) / maxDetectionRadius,0,1);
		while (i < 1.0f)
		{
			i += Time.deltaTime * rate;
			indicatorArrow.color = Color.Lerp(startColor, endColor, i);
			yield return null;
		}
		Destroy(indicatorArrow.gameObject);
		yield return null;
	}
}