using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AssignerTrigger : MonoBehaviour
{
    public ColorAssigner assigner;

    [SerializeField] [Range(-360f, 360f)] private float hue;
    [SerializeField] [Range(-1f, 1f)] private float sat;
    [SerializeField] [Range(-1f, 1f)] private float val;
    [SerializeField] [Range(-1f, 1f)] private float alpha;

    private Color curr_color;
    public bool oneuse = true;
    private bool inuse = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            inuse = true;
            Activate();
        }
    }

    void Activate()
    {
        assigner.hue = hue;
        assigner.sat = sat;
        assigner.val = val;
        assigner.alpha = alpha;

        Color tempColor = assigner.ColorReference.channelcolor;
        //assigner.ColorReference.Set(Color.white);
        assigner.ColorReference.Set(tempColor);

        if (oneuse)
        {
            Destroy(gameObject);
        }

        inuse = false;
    }

    public void Stop()
    {
        StopAllCoroutines();
        inuse = false;
    }
}
