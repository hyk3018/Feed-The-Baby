using System.Collections;
using System.Collections.Generic;
using FeedTheBaby;
using UnityEngine;

public class GoalLightAnimation : MonoBehaviour
{
    [SerializeField] Goals goals = null;

    [SerializeField] int tierToGlow = 0;

    [SerializeField] ParticleSystem[] glowParticles = null;

    Animator _animator;
    static readonly int Show = Animator.StringToHash("Show");

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        goals.TierFilled += StartPoof;
    }

    void StartPoof(int tier)
    {
        if (tier == tierToGlow)
        {
            _animator.SetTrigger(Show);
            foreach (var glowParticle in glowParticles)
                glowParticle.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}