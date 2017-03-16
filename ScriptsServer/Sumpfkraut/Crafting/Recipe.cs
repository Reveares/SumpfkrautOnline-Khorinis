﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
/* DB Structure
    Tabelle: Recipe

    ID -int
        eindeutige identifikation

    Codename - string
        Zur Identifikation für uns
        
    RecipeStep - int
        Jedes Rezept kann aus mehreren aufeinandernfolgenden Schritten bestehen

    NecessaryProperties - string
        Eigenschaften die ein Vob besitzen muss, damit an diesem Vob das Rezept ausgeführt werden kann

    Conditions
        Bedingungen die zu erfüllen sind, damit das Rezept verfügbar ist

    TimeToCraft - int
        Zeit in ms wie lange die Crafting animation abgespielt werden muss
        (Bei Minigame eventuell irrelevant)

    MinigameId - short
        Falls es ein Minispiel gibt, hier die ID dazu

    Euductlist - string
        Liste an nötigen Gegenständen für das Crafting
        Wie wird festgehalten wv verbraucht wird, bzw. bei einer Zange garnicht oder nur Haltbarkeit und ähnliches
        => lösung: haltbarkeitsitem wird als educt angegeben, removed und mit neuer haltbarkeit in der productliste angegeben
        #wird geparst#

    Productlist - string
        Liste an Gegenständen die durch das Crafting erschaffen werden
        Notation für change values überlegen
        #wird geparst#
    
    Effects - string
        Liste an effekten die nach dem crafting auf den spieler einwirken
        bsp, vergiftung, leben mehr/weniger, ausdauer etc, learning by doing effect etc.
        #wird geparst#

    CanCancel
        Regelung was passiert, wenn der der Vorgang abgebrochen wird bevor das Crafting vollendet ist
            - Edukte zurückerhalten - Fortschritt an item behalten
            - Edukte zurückerhalten - Fortschritt an item verloren
            - Edukte verlieren

    Default NULL - ansonsten
    

*/


namespace GUC.Scripts.Sumpfkraut.Crafting
{
    class Recipe
    {

        public int uniqueID;
        public string description;
        
        public Recipe(int ID, string codename, int step)
        {
            // create item use actions, create item create actions, create effects
            uniqueID = ID;
        }

        public bool CheckRequiredConditions(NPCInst craftsmen)
        {
            return true;
        }

        public bool CheckRequiredMaterials(NPCInst craftsmen)
        {
            return true;
        }

        public void CreateProducts(NPCInst craftsmen)
        {
            
        }

        public void ApplyEffects(NPCInst craftsmen)
        {

        }
    }
}
