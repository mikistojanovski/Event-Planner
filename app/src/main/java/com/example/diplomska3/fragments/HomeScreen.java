package com.example.diplomska3.fragments;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.DefaultItemAnimator;
import androidx.recyclerview.widget.DividerItemDecoration;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.app.ProgressDialog;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.example.diplomska3.R;
import com.example.diplomska3.adapter.GetEvents;
import com.example.diplomska3.ui.theme.MyViewHolder;
import com.example.diplomska3.viewmodels.EventDTO;
import com.firebase.ui.database.FirebaseRecyclerAdapter;
import com.firebase.ui.database.FirebaseRecyclerOptions;
import com.google.firebase.FirebaseOptions;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.ktx.Firebase;


import java.util.ArrayList;
import java.util.List;

public class HomeScreen extends Fragment {


    private View view;
    private RecyclerView recentPosts;

    private FirebaseRecyclerOptions<EventDTO> options;
    private FirebaseRecyclerAdapter<EventDTO, MyViewHolder> adapter;
    private DatabaseReference donor_ref,ref;
    FirebaseAuth mAuth;
    private GetEvents restAdapter;
    private List<EventDTO> postLists;
    private ProgressDialog pd;

    public HomeScreen() {

    }

    public View onCreate(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState){
        view = inflater.inflate(R.layout.activity_home_screen, container, false);
        recentPosts = (RecyclerView) view.findViewById(R.id.recyleposts);
        recentPosts.setHasFixedSize(true);
        recentPosts.setLayoutManager(new LinearLayoutManager(getContext()));

        ref=FirebaseDatabase.getInstance().getReference().child("events");

    donor_ref = FirebaseDatabase.getInstance().getReference();
    postLists = new ArrayList<>();

    pd = new ProgressDialog(getActivity());
        pd.setMessage("Loading...");
        pd.setCancelable(true);
        pd.setCanceledOnTouchOutside(false);

    mAuth = FirebaseAuth.getInstance();
    getActivity().setTitle("Events");

    restAdapter = new GetEvents(postLists);
    RecyclerView.LayoutManager pmLayout = new LinearLayoutManager(getContext());
        recentPosts.setLayoutManager(pmLayout);
        recentPosts.setItemAnimator(new DefaultItemAnimator());
        recentPosts.addItemDecoration(new DividerItemDecoration(getActivity(), LinearLayoutManager.VERTICAL));
        recentPosts.setAdapter(restAdapter);
        AddPosts();





 /*options = new FirebaseRecyclerOptions.Builder<EventDTO>().setQuery(ref, EventDTO.class).build();
adapter=new FirebaseRecyclerAdapter<EventDTO, MyViewHolder>(options) {
    @Override
    protected void onBindViewHolder(@NonNull MyViewHolder holder, int position, @NonNull EventDTO model) {

        holder.Title.setText("Title: "+ model.getTitle());
        holder.Genre.setText("Genre: "+ model.getGenre());
        holder.Price.setText("Price: "+ model.getPrice());
        holder.HostName.setText("Host: "+ model.getHostName());
        holder.Location.setText("Location: "+ model.getLocation());
        holder.Date.setText("Date: "+ model.getDate());
        holder.Time.setText("Time: "+ model.getTime());

    }

    @NonNull
    @Override
    public MyViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View v =LayoutInflater.from(parent.getContext()).inflate(R.layout.request_list_item,parent,false);

        return new MyViewHolder(v);
    }
};

adapter.startListening();
recentPosts.setAdapter(adapter);
 */
        return view;

}
    private void AddPosts()
    {
        Query allposts = donor_ref.child("events");
        pd.show();
        allposts.addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {

                if(dataSnapshot.exists()) {

                    for (DataSnapshot singlepost : dataSnapshot.getChildren()) {
                        EventDTO eventDTO = singlepost.getValue(EventDTO.class);
                        postLists.add(eventDTO);
                        restAdapter.notifyDataSetChanged();
                    }
                    pd.dismiss();
                }
                else
                {
                    Toast.makeText(getActivity(), "Database is empty now!",
                            Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {

                Log.d("User", databaseError.getMessage());

            }
        });

    }

    @Override
    public void onResume() {
        super.onResume();
    }

    @Override
    public void onStop() {
        super.onStop();
    }

    @Override
    public void onPause() {
        super.onPause();
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }
}