using UnityEngine;

public class ShantyCoordinator : MonoBehaviour
{
    private YarkinShanty[] _yarkinShanties;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _yarkinShanties = GetComponentsInChildren<YarkinShanty>();
    }

    float _time = 0;
    bool _playing = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_playing)
        {
            foreach (YarkinShanty yarkinShantyPlayer in _yarkinShanties)
            {
                yarkinShantyPlayer.UpdatePlayer(Time.fixedDeltaTime);
            }
            return;
        }

        _time += Time.deltaTime;
        if (_time > 3)
        {
            foreach(YarkinShanty yarkinShantyPlayer in _yarkinShanties)
            {
                _playing = true;
                yarkinShantyPlayer.PlayShantyOneShot();
            }
        }
    }
}
