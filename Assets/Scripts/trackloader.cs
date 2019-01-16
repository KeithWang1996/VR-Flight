using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class trackloader : MonoBehaviour {
    string trackdir = "Assets/Tracks/track2.txt";
    public GameObject checkpoint;
    GameObject curr_point;
    LineRenderer line;
    LineRenderer line2;
    GameObject waypoint;
    List<GameObject> waypoints;
    public AudioClip crush;
    public AudioClip beep1;
    public AudioClip beep2;
    public AudioClip beep3;
    public AudioClip fin;
    public AudioClip sonar;
    AudioSource source;
    AudioSource radar;
    public Material mat;
    public Material mat2;
    public GameObject navigator;
    public GameObject player;
    public GameObject Arrow;
    public Text text;//counter, go, finish
    public Text text2;//time in second;
    public Text text3;//lap 
    public Text text4;//distance
    int index;
    public static bool gamestart;
    bool gameover;
    bool timerstart;
    bool startsound;
    float countdown = 3;
    int i_countdown = 3;
    int radar_counter = 100;
    float timer;
    Quaternion a_rotate;
	// Use this for initialization
	void Start () {
        index = 1;
        timer = 0;
        waypoints = new List<GameObject>();
        StreamReader reader = new StreamReader(trackdir);
        source = GetComponent<AudioSource>();
        string temp;
        string[] xyz_s;
        Vector3 xyz;
        while ((temp = reader.ReadLine()) != null)
        {
            xyz_s = temp.Split(' ');
            xyz.x = float.Parse(xyz_s[0]) / 39.37f;
            xyz.y = float.Parse(xyz_s[1]) / 39.37f;
            xyz.z = float.Parse(xyz_s[2]) / 39.37f;
            waypoint = Instantiate(checkpoint, xyz, checkpoint.transform.rotation);
            waypoints.Add(waypoint);
        }
        curr_point = waypoints[index];
        player.transform.position = waypoints[0].transform.position;
        Vector3 pos = waypoints[1].transform.position;
        pos.y = 0;
        player.transform.LookAt(pos);
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.positionCount = waypoints.Count;
        line2 = navigator.AddComponent<LineRenderer>();
        line2.startWidth = 0.1f;
        line2.endWidth = 0.1f;
        line2.positionCount = 2;
        line.material = mat;
        line2.material = mat2;
        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, waypoints[i].transform.position);
        }
        gamestart = false;
        gameover = false;
        timerstart = false;
        startsound = false;
        text.text = "Ready?";
        text3.text = "0/" + (waypoints.Count-1).ToString();
        a_rotate = Arrow.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (line2 != null)
        {
            line2.SetPosition(0, navigator.transform.position);
            line2.SetPosition(1, curr_point.transform.position);
            Arrow.transform.LookAt(curr_point.transform.position);
            Arrow.transform.rotation = a_rotate * Arrow.transform.rotation;
            if (gamestart)
            {
                text4.text = Vector3.Distance(navigator.transform.position, curr_point.transform.position).ToString("0.00") + "m";
            }
            else
            {
                text4.text = "";
            }
        }
        else
        {
            text4.text = "";
        }

        if(countdown >= -1 && gesture.countstart)
        {
            countdown -= Time.deltaTime;
            if (!startsound)
            {
                startsound = true;
                text.text = "3";
                source.PlayOneShot(beep1);
            }
        }
        if(countdown <= i_countdown - 1 && i_countdown > 0)
        {
            i_countdown -= 1;
            text.text = i_countdown.ToString();
            if (i_countdown >= 1)
            {
                source.PlayOneShot(beep1);
            }
            else
            {
                source.PlayOneShot(beep2);
            }
        }
        if(i_countdown <= 0)
        {
            text.text = "";
            gamestart = true;
            timerstart = true;
        }
        if (timerstart && !gameover)
        {
            timer += Time.deltaTime;
            text2.text = timer.ToString("0.00") + "s";
        }
        if(countdown <= -1)
        {
            if (gameover)
            {
                text.text = "Fin!";
            }
            else
            {
                text.text = "";
            }
        }
        text3.text = (index-1).ToString() + "/" + (waypoints.Count - 1).ToString();
        /*
        radar_counter--;
        if (radar_counter <= 0)
        {
            radar = curr_point.GetComponent<AudioSource>();
            radar.volume = 1.0f;
            radar.loop = true;
            if (!radar.isPlaying)
                radar.PlayOneShot(sonar);
            radar_counter = 100;
        }
        */
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "check")
        {
            if(collision.gameObject == curr_point)
            {
                index ++;
                if (index < waypoints.Count - 1)
                {
                    curr_point = waypoints[index];
                }
                print(index);
            }
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "check")
        {
            if (other.gameObject == curr_point)
            {
                index++;
                if (index < waypoints.Count)
                {
   
                    curr_point = waypoints[index];
                    print(index);
                    source.PlayOneShot(beep3);
                }
                else
                {
                    Destroy(line2);
                    gameover = true;
                    source.PlayOneShot(fin);
                }
                
            }
        }
        else
        {
            if (!gameover) {
                source.PlayOneShot(crush);
                player.transform.position = waypoints[index - 1].transform.position;
                gamestart = false;
                text.text = "3";
                countdown = 3;
                i_countdown = 3;
                Vector3 pos = waypoints[index].transform.position;
                pos.y = 0;
                player.transform.LookAt(pos);
                //print("crush");
            }
        }
    }
}
