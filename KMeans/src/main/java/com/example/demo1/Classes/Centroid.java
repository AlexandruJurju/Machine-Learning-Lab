package com.example.demo1.Classes;

import javafx.scene.paint.Color;

import java.util.ArrayList;


public class Centroid extends Dot {

    private Color color;
    private ArrayList<Point> pointArrayList;

    public Centroid(int x, int y, Color color) {
        super(x, y);
        this.color = color;
        pointArrayList = new ArrayList<>();
    }

    public Color getColor() {
        return color;
    }

    public void setColor(Color color) {
        this.color = color;
    }

    public ArrayList<Point> getPointArrayList() {
        return pointArrayList;
    }

    public void setPointArrayList(ArrayList<Point> pointArrayList) {
        this.pointArrayList = pointArrayList;
    }
}
