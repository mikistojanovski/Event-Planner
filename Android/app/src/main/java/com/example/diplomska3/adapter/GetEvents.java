package com.example.diplomska3.adapter;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.diplomska3.R;
import com.example.diplomska3.viewmodels.EventDTO;
import com.squareup.picasso.Picasso;

import java.util.List;

public class GetEvents extends RecyclerView.Adapter<GetEvents.PostHolder> {



    private List<EventDTO> postLists;

    public class PostHolder extends RecyclerView.ViewHolder
    {
        TextView Title;
        TextView Genre;
        TextView Price;
        TextView HostName;
        TextView Location;
        TextView Date;
        TextView Time;

        ImageView Poster;



        public PostHolder(@NonNull View itemView) {
            super(itemView);

            Title = itemView.findViewById(R.id.title);
            Genre = itemView.findViewById(R.id.genre);
            Price = itemView.findViewById(R.id.price);
            HostName = itemView.findViewById(R.id.host);
            Location = itemView.findViewById(R.id.location);
            Date = itemView.findViewById(R.id.date);
            Time = itemView.findViewById(R.id.time);
            Poster = itemView.findViewById(R.id.poster);
        }
    }
    public GetEvents(List<EventDTO> postLists)
    {
        this.postLists = postLists;
    }

    @Override
    public PostHolder onCreateViewHolder(ViewGroup viewGroup, int i) {

        View listitem = LayoutInflater.from(viewGroup.getContext())
                .inflate(R.layout.request_list_item, viewGroup, false);

        return new PostHolder(listitem);
    }

    @Override
    public void onBindViewHolder(PostHolder postHolder, int i) {

        if(i%2==0)
        {
            postHolder.itemView.setBackgroundColor(Color.parseColor("#C13F31"));
        }
        else
        {
            postHolder.itemView.setBackgroundColor(Color.parseColor("#FFFFFF"));
        }

        EventDTO eventDTO = postLists.get(i);
        postHolder.Title.setText(eventDTO.getTitle());
        postHolder.Genre.setText("Genre: "+ eventDTO.getTitle());
        postHolder.Price.setText("Price: "+ eventDTO.getTitle());
        postHolder.HostName.setText("Host: "+ eventDTO.getTitle());
        postHolder.Location.setText("Location: "+ eventDTO.getTitle());
        postHolder.Date.setText("Date: "+ eventDTO.getTitle());
        postHolder.Time.setText("Time: "+ eventDTO.getTitle());
        Picasso.get().load(eventDTO.getPoster()).into(postHolder.Poster);
    }

    @Override
    public int getItemCount() {
        return postLists.size();
    }
}
