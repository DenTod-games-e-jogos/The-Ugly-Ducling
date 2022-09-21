using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelMaster;

public class MapGenerator : BaseGeneration
{
    [SerializeField] int mapLimit;
    [SerializeField] int nBiomes;
    [SerializeField] int frontier;
    [SerializeField] float noiseScale;

    //Variáveis auxiliares
    float radio;
    float radio2;
    short endWall = 7;
    short bioma1 = 2;
    short bioma2 = 5;
    short bioma3 = 0;
    short bioma4 = 7;
    //Value for blocks
    short air = -1;
    short desertoGrass = 0;
    short desertoDirty = 1;
    short florestaGrass = 2;
    short florestaDirty = 3;
    short florestaRock = 4;
    short pantanoGrass = 5;
    short pantanoDirty = 6;
    short planiceGrass = 7;


    void start()
    {
        endWall = planiceGrass;
        bioma1 = florestaGrass;
        bioma2 = pantanoGrass;
        bioma3 = desertoGrass;
        bioma4 = planiceGrass;
    }
    public override short Generation(int x, int y, int z) // The generation action only takes coordinates, you must return a block ID. 
    {
        //Calcula o raio
        radio = (x * x) + (z * z);
        radio2 = Mathf.Sqrt(radio);

        //Se está além do limite cosntroi um muro
        //End Wall
        if (radio2 > mapLimit)
        {
            return endWall;
        }
        //Interface entre o final e o ultimo bioma
        if ((radio2 <= mapLimit) && (radio2 > mapLimit - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return planiceGrass;
            }
            else if (y == 0)
            {
                return bioma4;
            }
        }

        //Se não for chão retora ar
        if (y != 0)
        {
            return air;
        }

        //Quarto Bioma
        //Planicie Sombria
        if ((radio2 <= mapLimit - frontier) && (radio2 > (3 * mapLimit / 4) + frontier))
        {
            return bioma4;
        }
        ////Interface entre o Bioma 4 e o Bioma 3
        if ((radio2 <= (3 * mapLimit / 4) + frontier) && (radio2 > (3 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma4;
            }
            else
            {
                return bioma3;
            }
        }

        //Terceiro Bioma
        //Deserto Vermelho
        if ((radio2 <= (3 * mapLimit / 4) - frontier) && (radio2 > (2 * mapLimit / 4) + frontier))
        {
            return bioma3;
        }
        ////Interface entre o Bioma 3 e o Bioma 2
        if ((radio2 <= (2 * mapLimit / 4) + frontier) && (radio2 > (2 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma3;
            }
            else
            {
                return bioma2;
            }
        }

        //Segundo Bioma
        //Mangue Roxo
        if ((radio2 <= (2 * mapLimit / 4) - frontier) && (radio2 > (1 * mapLimit / 4) + frontier))
        {
            return bioma2;
        }
        ////Interface entre o Bioma 2 e o Bioma 1
        if ((radio2 <= (1 * mapLimit / 4) + frontier) && (radio2 > (1 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma2;
            }
            else
            {
                return bioma1;
            }
        }

        //Prmeiro Bioma
        //Floresta Escura
        if (radio2 <= (1 * mapLimit / 4) - frontier)
        {
            return bioma1;
        }

        //Para qualquer outra situação não apresentada acima
        return air;
    }

    void Python()
    {
     //# End Wall = 5
     //   if (radio2 > maxRadio):
     // map[i, j] = 5
     //if ((radio2 <= maxRadio) and(radio2 > maxRadio - frontier)):
     // if (noise[i, j] >= 0):
     //   map[i, j] = 5
     // else:
     //   map[i, j] = 4
     //# Planicie Sombria = 4
     //if ((radio2 <= maxRadio - frontier) and(radio2 > (3 * maxRadio / 4) + frontier)):
     //   map[i, j] = 4
     //if ((radio2 <= (3 * maxRadio / 4) + frontier) and(radio2 > (3 * maxRadio / 4) - frontier)):
     // if (noise[i, j] >= 0):
     //   map[i, j] = 4
     // else:
     //   map[i, j] = 3
     //# Deserto Vermelho = 3
     //if ((radio2 <= (3 * maxRadio / 4) - frontier) and(radio2 > (2 * maxRadio / 4) + frontier)):
     //   map[i, j] = 3
     //if ((radio2 <= (2 * maxRadio / 4) + frontier) and(radio2 > (2 * maxRadio / 4) - frontier)):
     // if (noise[i, j] >= 0):
     //   map[i, j] = 3
     // else:
     //   map[i, j] = 2
     //# Mangue Roxo = 2
     //if ((radio2 <= (2 * maxRadio / 4) - frontier) and(radio2 > (1 * maxRadio / 4) + frontier)):
     //   map[i, j] = 2
     //if ((radio2 <= (1 * maxRadio / 4) + frontier) and(radio2 > (1 * maxRadio / 4) - frontier)):
     // if (noise[i, j] >= 0):
     //   map[i, j] = 2
     // else:
     //   map[i, j] = 1
     //# Floresta Escura = 1
     //if (radio2 <= (1 * maxRadio / 4) - frontier):
     //   map[i, j] = 1

    }
}