using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class gesture : MonoBehaviour {
    Hand lefthand;
    Hand righthand;
    Vector3 l_position;
    Vector3 r_position;
    Frame frame;
    float speed;
    Controller controller;
    Vector3 midpoint0;
    Vector3 wheel0;
    bool triggered;
    public static bool countstart;
    public AudioClip engine;
    public GameObject planemodel;
    public GameObject planemodel2;
    AudioSource source;
    bool start;
    int counter;
    int switchcount;
    // Use this for initialization
    void Start () {
        controller = new Controller();
        switchcount = 100;
        frame = controller.Frame();
        speed = 0;
        start = false;
        triggered = false;
        //triggering = false;
        midpoint0 = Vector3.zero;
        //lefthand = hands[0];
        //righthand = hands[2];
        counter = 30;
        countstart = false;
        source = GetComponent<AudioSource>();
        source.volume = 0.3f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        controller = new Controller();
        frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        speed = 0;
        if (hands.Count == 2) {
            counter = 30;
            lefthand = hands[0];
            righthand = hands[1];
            if (lefthand.IsRight)
            {
                righthand = hands[0];
                lefthand = hands[1];
            }
            List<Finger> fingerlist1 = lefthand.Fingers;
            List<Finger> fingerlist2 = righthand.Fingers;
            int left_extend = 0;
            int right_extend = 0;
            for(int i = 0; i < fingerlist1.Count; i++)
            {
                if (fingerlist1[i].IsExtended)
                {
                    left_extend++;
                }
            }

            for (int i = 0; i < fingerlist2.Count; i++)
            {
                if (fingerlist2[i].IsExtended)
                {
                    right_extend++;
                }
            }

            if (left_extend == 0 && right_extend != 0)
            {
                //print("leftfist");
                switchcount--;
                if (switchcount <= 0)
                {
                    planemodel.active = false;
                    planemodel2.active = false;
                    switchcount = 100;
                }
            }
            else if(left_extend != 0 && right_extend == 0)
            {
                //print("rightfist");
                switchcount--;
                if (switchcount <= 0)
                {
                    planemodel.active = true;
                    planemodel2.active = false;
                    switchcount = 100;
                }
            }
            else if(left_extend == 0 && right_extend == 0)
            {
                //print("bothfist");
                switchcount--;
                if (switchcount <= 0)
                {
                    planemodel.active = false;
                    planemodel2.active = true;
                    switchcount = 100;
                }
            }
            Vector diff = lefthand.PalmPosition - righthand.PalmPosition;
            //print(diff.Magnitude);
            Vector midpointv = lefthand.PalmPosition - 0.5f * diff;
            if (diff.Magnitude <= 150) { 
                start = true;
                countstart = true;
            }
            if (diff.Magnitude > 150 && start &&trackloader.gamestart)
            {
                
                speed = (diff.Magnitude - 150.0f) / 100.0f;
                if (!triggered)
                {
                    triggered = true;
                    midpoint0 = new Vector3(midpointv.x, midpointv.y, midpointv.z);
                    midpoint0.x = 0;
                    midpoint0 = Vector3.Normalize(midpoint0);
                    wheel0 = new Vector3(diff.x, diff.y, diff.z);
                    wheel0.z = 0;
                    wheel0 = Vector3.Normalize(wheel0);
                }
                Vector3 midpoint =new Vector3(midpointv.x, midpointv.y, midpointv.z);
                midpoint.x = 0;
                midpoint = Vector3.Normalize(midpoint);
                Vector3 wheel = new Vector3(diff.x, diff.y, diff.z);
                wheel.z = 0;
                wheel = Vector3.Normalize(wheel);
                if (triggered)
                {
                    float angle = (Vector3.SignedAngle(midpoint0, midpoint, Vector3.right)) / 20.0f;
                    transform.parent.parent.parent.Rotate(new Vector3(angle, 0, 0));
                    float angle2 = (Vector3.SignedAngle(wheel0, wheel, Vector3.back)) / 20.0f;
                    transform.parent.parent.parent.Rotate(new Vector3(0, angle2, 0));
                }
            }
            else
            {
                triggered = false;
            }
            
            //Vector3 ydirection = Vector3.Normalize(midpoint - transform.parent.parent.parent.position);
            //float angle = Vector3.Angle(midpoint, transform.parent.parent.parent.forward);
            //transform.parent.parent.parent.rotation *= angle;
        }
        else
        {
            start = false;
        }
        if (speed != 0 && hands.Count == 2 && trackloader.gamestart)
        {
            source.pitch = speed * 0.5f;
            source.PlayOneShot(engine);
        }
        else
        {
            source.Stop();
        }
        transform.parent.parent.parent.position += transform.parent.parent.parent.forward * speed;
        
    }
}
