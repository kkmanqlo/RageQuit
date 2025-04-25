using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Shuriken : MonoBehaviour
{
 [System.Serializable]
    public class ShurikenData
    {
        public GameObject shurikenPrefab;
        public Vector3 startPosition;
        public Vector3 direction;
        public float distance = 5f;
        public float duration = 1f;
        public bool destroyOnEnd = true;
    }

    public List<ShurikenData> shurikensToThrow;

    void Start()
    {
        foreach (ShurikenData data in shurikensToThrow)
        {
            LaunchShuriken(data);
        }
    }

    void LaunchShuriken(ShurikenData data)
    {
        GameObject shuriken = Instantiate(data.shurikenPrefab, data.startPosition, Quaternion.identity);

        Vector3 endPosition = data.startPosition + data.direction.normalized * data.distance;

        shuriken.transform.DOMove(endPosition, data.duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (data.destroyOnEnd)
                    Destroy(shuriken);
            });
    }
}
