using UnityEngine;
using System.Collections.Generic;

public class Presentation : MonoBehaviour
{
    public SpriteRenderer SlideRender;
    public List<Sprite> Slides;
    public int CurrentSlideIndex;

    void Start()
    {
        CurrentSlideIndex = 1;
        SlideRender = GetComponent<SpriteRenderer>();
    }
	

	void Update () 
    {
	    if (Input.GetMouseButtonDown(0))
	    {
            CurrentSlideIndex++;
            if (CurrentSlideIndex >= Slides.Count)
	        {
	            Application.LoadLevel(1);
	            return;
	        }
	        SlideRender.sprite = Slides[CurrentSlideIndex];
	    }
	}
}
