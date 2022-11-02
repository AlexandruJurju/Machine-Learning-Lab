package com.example.demo1.Classes;

public class Point extends Dot {

    private int zone;

    public Point(int x, int y, int zone) {
        super(x, y);
        this.zone = zone;
    }

    public int getZone() {
        return zone;
    }

    public void setZone(int zone) {
        this.zone = zone;
    }
}
