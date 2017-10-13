using UnityEngine;
using System.Collections;

public class Places {

    public int idPlace; //ID DO LOCAL
    public string placeDescription; //Descricao do local
    public string placeName; //Nome do local
    public int hasItem; //Tem item? qual item? 0 = NULL

    public int needItemToPass;

    public int hasDoorClose; //Tem porta fechada? 0 = NULL
    public int doorCloseItem; //ID ITEM PARA ABRIR A PORTA

    public int north = 0; // Norte
    public int south = 0; // Sul
    public int east = 0;  // Leste
    public int west = 0;  // Oeste

    public Places(int newidPlace, string newPlaceDesc, string newplaceName,int newHasItem, int newHasDoorClose, int newDoorCloseItem, int newNeedItemToPass, int newNorth, int newSouth, int newEast, int newWest)
    {
        idPlace = newidPlace;
        placeDescription = newPlaceDesc;
        placeName = newplaceName;
        hasItem = newHasItem;
        hasDoorClose = newHasDoorClose;
        doorCloseItem = newDoorCloseItem;
        needItemToPass = newNeedItemToPass;      
               
        north = newNorth;
        south = newSouth;
        east = newEast;
        west = newWest;
    }
}
