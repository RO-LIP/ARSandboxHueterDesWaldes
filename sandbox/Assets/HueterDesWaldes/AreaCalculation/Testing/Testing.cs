using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.HueterDesWaldes.AreaCalculation
{
    public class Testing : MonoBehaviour
    {
        float time = 0;
        float delay = 1;
        bool testsHaveBeenRun = false;
        bool printedFinalMessage = false;
        GameManager gameManager = null;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Preparing to start Tests");
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (!testsHaveBeenRun)
            {
                if (time >= delay)
                {
                    Debug.Log("Tests started");

                    //PUT YOUR TESTS BOLOW THIS COMMENT

                    TestTile();
                    TestArea();
                    TestAreaCalculator();

                    //PUT YOUR TESTS ABOVE THIS COMMENT


                    testsHaveBeenRun = true;
                }
            }
            else if (!printedFinalMessage)
            {
                Debug.Log("Tests finished in " + time + "seconds");
                printedFinalMessage = true;
            }
        }

        private void TestTile()
        {
            Debug.Log(nameof(TestTile));
            Vector3 posA = new Vector3(1, 1, 2);
            HeightLevel heightLevelA = new HeightLevel();
            Tile a = new Tile(posA, 0, 0, heightLevelA, 1);

            Vector3 posB = new Vector3(1, 1, 2);
            HeightLevel heightLevelB = new HeightLevel();
            Tile b = new Tile(posB, 0, 0, heightLevelB, 1);

            Vector3 posC = new Vector3(0, 1, 2);
            HeightLevel heightLevelC = new HeightLevel();
            Tile c = new Tile(posC, 0, 0, heightLevelC, 1);

            Vector3 posD = new Vector3(2, 1, 2);
            HeightLevel heightLevelD = new HeightLevel();
            Tile d = new Tile(posD, 0, 0, heightLevelD, 1);

            Vector3 posE = new Vector3(1, 1, 1);
            HeightLevel heightLevelE = new HeightLevel();
            Tile e = new Tile(posE, 0, 0, heightLevelE, 1);

            Vector3 posF = new Vector3(1, 1, 3);
            HeightLevel heightLevelF = new HeightLevel();
            Tile f = new Tile(posF, 0, 0, heightLevelF, 1);

            Assert("a=" + a.WorldPos + " b=" + b.WorldPos + " a == b", a.CompareTo(b) == 0);
            Assert("a=" + a.WorldPos + " c=" + c.WorldPos + " a > c", a.CompareTo(c) == 1);
            Assert("a=" + a.WorldPos + " d=" + d.WorldPos + " a < d", a.CompareTo(d) == -1);
            Assert("a=" + a.WorldPos + " e=" + e.WorldPos + " a > e", a.CompareTo(e) == 1);
            Assert("a=" + a.WorldPos + " f=" + f.WorldPos + " a < f", a.CompareTo(d) == -1);
        }

        private void TestArea()
        {
            Debug.Log(nameof(TestArea));
            //create empty area
            Area area = new Area();
            Assert("create empty area", area != null);

            int notFoundCount = 0;
            bool foundAllTiles = true;
            int bounds = 4;
            float max = 128;
            float min = -max;
            for (int i = 0; i < bounds; i++)
            {
                for (int j = 0; j < bounds; j++)
                {
                    Tile tile = null;
                    bool contains = true;
                    while (contains)
                    {
                        float x = Random.Range(min, max);
                        float y = Random.Range(min, max);
                        float z = Random.Range(min, max);
                        Vector3 pos = new Vector3(x, y, z);
                        HeightLevel heightLevel = new HeightLevel();
                        tile = new Tile(pos, 0, 0, heightLevel, 1);
                        contains = area.Contains(tile);
                        if (contains)
                            Debug.LogWarning("collision");
                    }
                    area.Add(tile);
                    if (!area.Contains(tile))
                    {
                        notFoundCount++;
                        foundAllTiles = false;
                    }
                }
            }
            int count = area.GetTiles().Count;
            Assert("Added " + bounds * bounds + " tiles to area. Count=" + count, count == (bounds * bounds));
            Assert("Searching for tiles. Number of Tiles not found=" + notFoundCount, foundAllTiles);

            area = new Area();
            bounds = 2;
            float tileSize = 1;
            for (int i = 0; i < bounds; i++)
            {
                for (int j = 0; j < bounds; j++)
                {
                    Tile tile = null;
                    //bool contains = true;
                    //while (contains)
                    //{
                    float x = Random.Range(min, max);
                    float y = Random.Range(min, max);
                    float z = Random.Range(min, max);
                    Vector3 pos = new Vector3(x, y, z);
                    HeightLevel heightLevel = new HeightLevel();
                    tile = new Tile(pos, 0, 0, heightLevel, tileSize);
                    //    contains = area.Contains(tile);
                    //    if (contains)
                    //        Debug.LogWarning("collision");
                    //}
                    area.Add(tile);
                }
            }
            float areaSize = area.GetTiles().Count * tileSize;
            Assert("Get size of area", area.SizeInSquareMeters() == areaSize);
        }

        private void TestAreaCalculator()
        {
            Debug.Log(nameof(TestAreaCalculator));
            Assert("Found GameManager", gameManager != null);
            if (gameManager == null)
                throw new Exception("game manager is null. Test aborted");

            AreaCalculator areaCalculator = FindObjectOfType<AreaCalculator>();
            Assert("Found AreaCalculator", areaCalculator != null);

            areaCalculator.Calculate();
        }

        private void Assert(string assertionText, bool value)
        {
            if (value)
                Debug.Log(assertionText + ":" + value);
            else
                Debug.LogError(assertionText + ":" + value);
        }
    }
}