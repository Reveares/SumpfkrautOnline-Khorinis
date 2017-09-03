﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUC.Types;
using GUC.Utilities;

namespace GUC.Scripts.Arena
{
    class TOTeamDef
    {
        string name;
        public string Name { get { return name; } }

        List<Vec3f, Vec3f> spawnPoints;
        public IEnumerable<Vec3f, Vec3f> SpawnPoints { get { return spawnPoints.AsEnumerable(); } }

        List<TOClassDef> classDefs;
        public IEnumerable<TOClassDef> ClassDefs { get { return classDefs; } }

        public TOTeamDef(string name, List<Vec3f, Vec3f> spawnPoints, List<TOClassDef> classDefs)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            if (spawnPoints == null || spawnPoints.Count == 0)
                throw new ArgumentNullException("spawnPoints");
            if (classDefs == null || classDefs.Count == 0)
                throw new ArgumentNullException("classDefs");

            this.name = name;
            this.spawnPoints = spawnPoints;
            this.classDefs = classDefs;
        }
    }
}