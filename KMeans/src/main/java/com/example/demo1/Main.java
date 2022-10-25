package com.example.demo1;

import javafx.application.Application;
import javafx.scene.Group;
import javafx.scene.Scene;
import javafx.scene.paint.Color;
import javafx.scene.shape.Circle;
import javafx.scene.shape.Line;
import javafx.stage.Stage;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Random;
import java.util.Scanner;

public class Main extends Application {

    private final int graphWidth = 600;
    private final int graphHeight = 600;
    private final int xOffset = 50;
    private final int yOffset = 50;
    private ArrayList<Color> colorList = new ArrayList<>();
    private Group root;

    private final Random random = new Random();

    public void drawAxis(int overflow) {
        Line oX = new Line();
        oX.setStartX(xOffset - overflow);
        oX.setStartY(graphHeight / 2.0 + yOffset);
        oX.setEndX(graphWidth + xOffset + overflow);
        oX.setEndY(graphHeight / 2.0 + yOffset);
        oX.setStroke(Color.WHITE);


        Line oY = new Line();
        oY.setStartX(graphWidth / 2.0 + xOffset);
        oY.setStartY(yOffset - overflow);
        oY.setEndX(graphWidth / 2.0 + xOffset);
        oY.setEndY(graphHeight + yOffset + overflow);
        oY.setStroke(Color.WHITE);

        root.getChildren().add(oX);
        root.getChildren().add(oY);
    }

    public void drawBorder() {
        int xLimit = 300 + graphWidth / 2;
        int yLimit = graphHeight / 2 - 300;

        Line topLine = new Line();
        topLine.setStartX(xLimit - 600 + xOffset);
        topLine.setStartY(yLimit + yOffset);
        topLine.setEndX(xLimit + xOffset);
        topLine.setEndY(yLimit + yOffset);
        topLine.setStroke(Color.WHITE);
        root.getChildren().add(topLine);

        Line bottomLine = new Line();
        bottomLine.setStartX(xLimit - 600 + xOffset);
        bottomLine.setStartY(yLimit + 600 + yOffset);
        bottomLine.setEndX(xLimit + xOffset);
        bottomLine.setEndY(yLimit + yOffset + 600);
        bottomLine.setStroke(Color.WHITE);
        root.getChildren().add(bottomLine);

        Line leftLine = new Line();
        leftLine.setStartX(xLimit - 600 + xOffset);
        leftLine.setStartY(yLimit + yOffset);
        leftLine.setEndX(xLimit - 600 + xOffset);
        leftLine.setEndY(yLimit + yOffset + 600);
        leftLine.setStroke(Color.WHITE);
        root.getChildren().add(leftLine);

        Line rightLine = new Line();
        rightLine.setStartX(xLimit + xOffset);
        rightLine.setStartY(yLimit + yOffset);
        rightLine.setEndX(xLimit + xOffset);
        rightLine.setEndY(yLimit + yOffset + 600);
        rightLine.setStroke(Color.WHITE);
        root.getChildren().add(rightLine);
    }

    public ArrayList<Point> readFile() throws FileNotFoundException {
        ArrayList<Point> output = new ArrayList<>();

        File file = new File("src/main/java/points.txt");

        Scanner sc = new Scanner(file);

        while (sc.hasNextLine()) {
            String line = sc.nextLine();

            String[] split = line.split(" ");

            output.add(new Point(Integer.parseInt(split[0]), Integer.parseInt(split[1]), Integer.parseInt(split[2])));
        }

        return output;
    }

    public Dot convertToScreen(Dot input) {
        int screenX = input.getX() + graphWidth / 2 + xOffset;
        int screenY = graphHeight / 2 - input.getY() + yOffset;

        return new Dot(screenX, screenY);
    }

    private void addColors() {
        colorList.add(Color.GREEN);
        colorList.add(Color.CYAN);
        colorList.add(Color.RED);
        colorList.add(Color.ALICEBLUE);
        colorList.add(Color.DARKORANGE);
        colorList.add(Color.PURPLE);
        colorList.add(Color.PINK);
        colorList.add(Color.BROWN);
        colorList.add(Color.TURQUOISE);
        colorList.add(Color.ORANGE);
    }

    private ArrayList<Centroid> generateCentroids() {
        ArrayList<Centroid> centroidList = new ArrayList<>();
        int centroidCount = random.nextInt(2, 10);
        for (int i = 0; i < centroidCount; i++) {
            int x = random.nextInt(-300, 300);
            int y = random.nextInt(-300, 300);
            centroidList.add(new Centroid(x, y, colorList.get(i)));
        }
        return centroidList;
    }

    private void drawCentroids(ArrayList<Centroid> centroids) {
        ArrayList<Circle> circleArrayList = new ArrayList<>();

        int i = 0;
        for (Centroid centroid : centroids) {
            int screenX = centroid.getX() + graphWidth / 2 + xOffset;
            int screenY = graphHeight / 2 - centroid.getY() + yOffset;

            Circle circle = new Circle();
            circle.setCenterX(screenX);
            circle.setCenterY(screenY);
            circle.setRadius(10);
            circle.setStroke(Color.INDIGO);
            circle.setStrokeWidth(5);
            circle.setFill(centroid.getColor());

            circleArrayList.add(circle);
        }
        root.getChildren().addAll(circleArrayList);
    }

    private void drawPointsFromCentroids(ArrayList<Centroid> centroids) {
        ArrayList<Circle> circleList = new ArrayList<>();

        for (Centroid centroid : centroids) {
            for (Point point : centroid.getPointArrayList()) {
                Circle circle = new Circle();
                Dot screen = convertToScreen(point);

                circle.setCenterX(screen.getX());
                circle.setCenterY(screen.getY());
                circle.setRadius(2);
                circle.setFill(centroid.getColor());

                circleList.add(circle);
            }
        }

        root.getChildren().addAll(circleList);
    }

    /*============================================*/
    double distance(Point point, Centroid centroid) {
        double deltaX = centroid.getX() - point.getX();
        double deltaY = centroid.getY() - point.getY();
        return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private void groupPoints(ArrayList<Point> pointArrayList, ArrayList<Centroid> centroidArrayList) {
        for (Point point : pointArrayList) {
            double minDistance = 10000;
            Centroid bestCentroid = null;

            for (Centroid centroid : centroidArrayList) {
                double distance = distance(point, centroid);

                if (distance < minDistance) {
                    minDistance = distance;
                    bestCentroid = centroid;
                }
            }
            bestCentroid.getPointArrayList().add(point);
        }
    }

    private Dot calculateCenterOfGravity(Centroid centroid) {
        int xSum = 0;
        int ySum = 0;
        for (Point point : centroid.getPointArrayList()) {
            xSum += point.getX();
            ySum += point.getY();
        }

        int xMean = xSum / centroid.getPointArrayList().size();
        int yMean = ySum / centroid.getPointArrayList().size();

        return new Dot(xMean, yMean);
    }

    private double convergence(Centroid centroid) {
        double Ec = 0;
        for (Point point : centroid.getPointArrayList()) {
            Ec += distance(point, centroid);
        }
        return Ec;
    }
    
    @Override
    public void start(Stage stage) throws IOException, InterruptedException {
        root = new Group();
        Scene scene = new Scene(root, 1250, 750, Color.BLACK);


        addColors();

        drawAxis(30);
        drawBorder();

        ArrayList<Point> points = readFile();
        ArrayList<Centroid> centroids = generateCentroids();

        groupPoints(points, centroids);
        drawPointsFromCentroids(centroids);
        drawCentroids(centroids);
        root.getChildren().clear();


        for (Centroid centroid : centroids) {
            Dot centerOfGravity = calculateCenterOfGravity(centroid);
            centroid.setX(centerOfGravity.getX());
            centroid.setY(centerOfGravity.getY());
        }
        drawAxis(30);
        drawBorder();
        groupPoints(points, centroids);
        drawPointsFromCentroids(centroids);
        drawCentroids(centroids);


        stage.setTitle("Main Window");
        stage.setScene(scene);
        stage.show();
    }


    public static void main(String[] args) {
        launch();
    }
}