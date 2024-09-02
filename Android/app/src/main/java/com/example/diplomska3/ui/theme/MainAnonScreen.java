package com.example.diplomska3.ui.theme;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.example.diplomska3.R;
import com.example.diplomska3.viewmodels.EventDTO;
import com.firebase.ui.database.FirebaseRecyclerAdapter;
import com.firebase.ui.database.FirebaseRecyclerOptions;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.squareup.picasso.Picasso;

public class MainAnonScreen extends AppCompatActivity {


    FloatingActionButton floatingActionButton,floatingActionButton2;
    RecyclerView recyclerView;
    FirebaseAuth mAuth;

    FirebaseRecyclerOptions<EventDTO> options;
    FirebaseRecyclerAdapter<EventDTO, MyViewHolder> adapter;
    DatabaseReference DataRef;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_anon_screen);

        floatingActionButton2=findViewById(R.id.fab2);
        DataRef = FirebaseDatabase.getInstance().getReference().child("events");
        recyclerView=findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(new LinearLayoutManager(getApplicationContext()));
        recyclerView.setHasFixedSize(true);



        floatingActionButton2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(getApplicationContext(),LoginScreen.class));
            }
        });

        LoadData();
    }

    private void LoadData() {
        options = new FirebaseRecyclerOptions.Builder<EventDTO>().setQuery(DataRef, EventDTO.class).build();
        adapter=new FirebaseRecyclerAdapter<EventDTO, MyViewHolder>(options) {
            @Override
            protected void onBindViewHolder(@NonNull MyViewHolder holder, int position, @NonNull EventDTO model) {

                if(position%2==0)
                {
                    holder.itemView.setBackgroundColor(Color.parseColor("#C13F31"));
                }
                else
                {
                    holder.itemView.setBackgroundColor(Color.parseColor("#FFFFFF"));
                }
                holder.Title.setText("Title: "+ model.getTitle());
                holder.Genre.setText("Genre: "+ model.getGenre());
                holder.Price.setText("Price: "+ model.getPrice());
                holder.HostName.setText("Host: "+ model.getHostName());
                holder.Location.setText("Location: "+ model.getLocation());
                holder.Date.setText("Date: "+ model.getDate());
                holder.Time.setText("Time: "+ model.getTime());
                Picasso.get().load(model.getPoster()).into(holder.Poster);
            }

            @NonNull
            @Override
            public MyViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
                View v= LayoutInflater.from(parent.getContext()).inflate(R.layout.request_list_item,parent,false);

                return new MyViewHolder(v);
            }
        };
        adapter.startListening();
        recyclerView.setAdapter(adapter);
    }
}