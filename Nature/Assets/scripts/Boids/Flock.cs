using UnityEngine;

public class Flock : MonoBehaviour
{
    public float minSpeed = 0.5f, maxSpeed = 1, rotationSpeed = 4, neighborDistance = 3;
    private Vector3 averageHeading, averagePosition;
    private float speed;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.size)
        {
            Vector3 direction = Vector3.zero - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((direction)),
                rotationSpeed * Time.deltaTime);
        }
        else if (Random.Range(0, 5) < 1)
            ApplyRules();

        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void ApplyRules()
    {
        GameObject[] fishes = GlobalFlock.fishes;

        Vector3 vcenter = Vector3.zero, vavoid = Vector3.zero, goalPos = GlobalFlock.goalPos;
        float groupSpeed = 0.1f;
        int groupSize = 0;

        foreach (GameObject fish in fishes)
        {
            if (fish != gameObject)
            {
                float dist = Vector3.Distance(fish.transform.position, transform.position);
                if (dist <= neighborDistance)
                {
                    vcenter += fish.transform.position;
                    groupSize++;
                    if (dist < 1)
                        vavoid += transform.position - fish.transform.position;
                    groupSpeed += fish.GetComponent<Flock>().speed;
                }
            }
        }

        if (groupSize <= 0) return;
        vcenter = vcenter / groupSize + (goalPos - transform.position);
        speed = groupSpeed / groupSize;
        Vector3 direction = (vcenter + vavoid) - transform.position;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }
}