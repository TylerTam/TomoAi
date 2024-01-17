using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] private FurnitureData furnitureData;
    


    public void InitFuriniture(Vector3 pos, float y, FurnitureData furnData)
    {
        transform.position = pos;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
    }
    public FurnitureConfig GetConfig()
    {
        FurnitureConfig conf = new FurnitureConfig();
        conf.FurnitureName = furnitureData.ItemId;
        conf.FurniturePos = transform.position;
        conf.FurinitureYRot = transform.eulerAngles.y;
        return conf;
    }
}
