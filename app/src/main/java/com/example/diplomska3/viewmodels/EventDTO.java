package com.example.diplomska3.viewmodels;

import java.io.Serializable;
import java.sql.Time;
import java.util.Date;
import java.util.Calendar;
import java.util.TimeZone;
public class EventDTO implements Serializable {
    private String Title,Genre,HostName,Location,Poster;
    private Integer Price;
    private String Date;
    private String Time;

    public EventDTO() {

    }

    public EventDTO(String title, String genre, String hostName, String location, String poster, Integer price, String date, String time) {
        Title = title;
        Genre = genre;
        HostName = hostName;
        Location = location;
        Poster = poster;
        Price = price;
        Date = date;
        Time = time;
    }



    public  String getTitle() {
        return Title;
    }

    public void setTitle(String title) {
        Title = title;
    }

    public  String getGenre() {
        return Genre;
    }

    public void setGenre(String genre) {
        Genre = genre;
    }

    public String getHostName() {
        return HostName;
    }

    public void setHostName(String hostName) {
        HostName = hostName;
    }

    public String getLocation() {
        return Location;
    }

    public void setLocation(String location) {
        Location = location;
    }

    public String getPoster() {
        return Poster;
    }

    public void setPoster(String poster) {
        Poster = poster;
    }

    public Integer getPrice() {
        return Price;
    }

    public void setPrice(Integer price) {
        Price = price;
    }

    public String getDate() {
        return Date;
    }

    public void setDate(String date) {
        Date = date;
    }

    public String getTime() {
        return Time;
    }

    public void setTime(String time) {
        Time = time;
    }
}
