using UnityEngine;
using System.Collections;

public class RockMovement : MonoBehaviour
{
    public Transform HandTransform { get; set; }
    public GameObject Hand { get; set; }

    public bool IsLifted { get; set; }

    private Rigidbody rb;
    private bool isLevitating = false;
    private bool followHandMovement = false;

    private float levitationForceMultiplier = 2f; // Multiplier to counteract gravity
    // private float maxLevitationHeight; // Maximum height the rock should levitate to, set to the hand's height on spawn

    private Vector3 lastHandPosition;

    
    public void Initialize(GameObject hand, float initialUpwardForce)
    {
        Hand = hand;
        HandTransform = hand.transform;
        rb = GetComponent<Rigidbody>();

        hand.GetComponent<HandGestureManager>().OnPinchRelease += StopFollowingHand;
        // maxLevitationHeight = HandTransform.position.y;

        StartCoroutine(LevitationRoutine());
    }

    void OnDestroy()
    {
        if (Hand != null)
        {
            Hand.GetComponent<HandGestureManager>().OnPinchRelease -= StopFollowingHand;
        }
    }

    private IEnumerator LevitationRoutine()
    {
        // While the rock is below the hand's height, apply upward force
        while (transform.position.y < HandTransform.position.y)
        {
            float forceToApply = Mathf.Abs(Physics.gravity.y) * rb.mass * levitationForceMultiplier;
            rb.AddForce(Vector3.up * forceToApply);
            yield return null;
        }

        // Once the rock reaches the hand's height, start following the hand's movement
        isLevitating = true;
        rb.velocity = Vector3.zero; // Stop any residual upward movement
        rb.useGravity = false; // Disable gravity
        followHandMovement = true;
        lastHandPosition = HandTransform.position;
        IsLifted = true;

        Debug.Log("Initialize");
    }

    private void FixedUpdate()
    {
        if (followHandMovement)
        {
            FollowHandMovement();
        }
        

        if(transform.position.y <= -2f)
            Destroy(gameObject);
    }

    private void FollowHandMovement()
    {
        // Calculate hand velocity
        Vector3 handVelocity = (HandTransform.position - lastHandPosition) / Time.fixedDeltaTime;

        // Apply the hand's horizontal velocity to the rock
        rb.velocity = 2.25f * new Vector3(handVelocity.x, handVelocity.y, handVelocity.z);
        
        // Ensure the rock stays at the hand's height by adjusting its vertical position without affecting its velocity
        // transform.position = new Vector3(transform.position.x, maxLevitationHeight, transform.position.z);

        lastHandPosition = HandTransform.position;
    }

    // Call this method to stop following the hand's movement and enable gravity again
    public void StopFollowingHand()
    {
        followHandMovement = false;
        
        if(rb != null)
            rb.velocity = Vector3.zero;

        StartCoroutine(DelayedRelease());
    }

    private IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(0.75f);

        isLevitating = false;
        IsLifted = false;
        rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(followHandMovement || !IsLifted)
        {
            return;
        }

        if (collision.gameObject.GetComponentInParent<HandCollider>())
        {
            float impactForceMultiplier = 3f;
            Vector3 impactVelocity = collision.relativeVelocity;
            Vector3 impactForce = impactVelocity * impactForceMultiplier;

            // impactForce.y = 0;

            // Apply the calculated force
            rb.AddForce(impactForce, ForceMode.Impulse);
            rb.useGravity = true;
        }
    }
}