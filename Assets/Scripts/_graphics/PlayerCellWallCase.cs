using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    ORDER:
      1
    6   2
    5   3
      4

    Ex: IIIOIO: 123_5_
     --
        \
    \   /
      
*/
public enum PlayerCellWallCase {
    IIOIII,
    OIOIOI,
    OIIOII,
    OIOIII,
    OIOOII, // this sprite is swapped with the other because of reasons
    IIIOOI,
    IIOOOI,
    OIIOOI, // this sprite is swapped with the other because of reasons
    OOOOOO

}
