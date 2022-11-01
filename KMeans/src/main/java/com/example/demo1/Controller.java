package com.example.demo1;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class Controller {

    public double roundAvoid(double value, int places) {
        double scale = Math.pow(10, places);
        return Math.round(value * scale) / scale;
    }

    public double gauss(int coordinate, int m, int sigma) {
        double gaussRez = Math.exp((double) -(((m - coordinate) * (m - coordinate)) / (2 * sigma * sigma)));
        return roundAvoid(gaussRez, 20);
    }

    public ArrayList<Point> getPoints(ArrayList<Zone> zoneList, int pointsToFind) {

        ArrayList<Point> output = new ArrayList<>();

        Random rand = new Random();

        boolean foundX = false;
        boolean foundY = false;
        int randZoneIndex;
        int xCoord = 0;
        int yCoord = 0;

        while (output.size() < pointsToFind) {

            randZoneIndex = rand.nextInt(zoneList.size());

            while (!foundX) {
                int x = rand.nextInt(-300, 300);

                double gaussRez = gauss(x, zoneList.get(randZoneIndex).getmX(), zoneList.get(randZoneIndex).getSigmaX());
                if (gaussRez > rand.nextDouble()) {
                    xCoord = x;
                    foundX = true;
                } else if (gaussRez == 0 && rand.nextDouble() < 0.00001) {
                    xCoord = x;
                    foundX = true;
                }
            }

            while (!foundY) {
                int y = rand.nextInt(-300, 300);

                double gaussRez = gauss(y, zoneList.get(randZoneIndex).getmY(), zoneList.get(randZoneIndex).getSigmaY());
                if (gaussRez > rand.nextDouble()) {
                    yCoord = y;
                    foundY = true;
                } else if (gaussRez == 0 && rand.nextDouble() < 0.00001) {
                    yCoord = y;
                    foundY = true;
                }
            }

            foundX = false;
            foundY = false;
            output.add(new Point(xCoord, yCoord, randZoneIndex));
        }

        return output;
    }

    public void writePoints(ArrayList<Point> points) throws IOException {
        File myFile = new File("src/main/java/points.txt");
        PrintWriter writer = new PrintWriter(new FileWriter(myFile));
        for (Point point : points) {
            writer.println(point.getX() + " " + point.getY() + " " + point.getZone());
        }
        writer.close();
    }

}
