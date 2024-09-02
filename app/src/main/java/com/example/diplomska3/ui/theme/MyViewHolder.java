package com.example.diplomska3.ui.theme;

import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.diplomska3.R;

import java.util.Date;

public class MyViewHolder extends RecyclerView.ViewHolder {
     TextView Title,Genre,HostName,Location;

     TextView Price;
     TextView Date;
     TextView Time;
     ImageView Poster;

    public MyViewHolder(@NonNull View itemView) {
        super(itemView);
        Title = itemView.findViewById(R.id.title);
        Genre = itemView.findViewById(R.id.genre);
        Price = itemView.findViewById(R.id.price);
        HostName = itemView.findViewById(R.id.host);
        Location = itemView.findViewById(R.id.location);
        Date = itemView.findViewById(R.id.date);
        Time = itemView.findViewById(R.id.time);
        Poster=itemView.findViewById(R.id.poster);

    }
}
