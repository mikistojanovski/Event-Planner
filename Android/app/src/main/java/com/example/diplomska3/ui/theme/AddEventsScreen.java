package com.example.diplomska3.ui.theme;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.app.ProgressDialog;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.Spinner;
import android.widget.Toast;

import com.example.diplomska3.R;
import com.example.diplomska3.viewmodels.UserData;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseUser;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.firebase.database.ValueEventListener;
import com.google.firebase.storage.FirebaseStorage;
import com.google.firebase.storage.OnProgressListener;
import com.google.firebase.storage.StorageReference;
import com.google.firebase.storage.UploadTask;

import java.util.Calendar;
import java.util.HashMap;

public class AddEventsScreen extends AppCompatActivity {

   ImageView imageView;
    ProgressDialog pd;
    static  final int REQUEST_CODE_IMAGE=101;
    Uri uri;
    boolean isImageAdded=false;
    EditText text1, text2,text3,text4,text5,text6,loc;
    Spinner spinner1;
    Button btnpost;

    StorageReference sref;
    FirebaseDatabase fdb;
    DatabaseReference db_ref,ref;
    FirebaseAuth mAuth;

    Calendar cal;
    String uid;
    String Time, Date;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_add_events_screen);

        pd = new ProgressDialog(this);
        pd.setMessage("Loading...");
        pd.setCancelable(true);
        pd.setCanceledOnTouchOutside(false);

//        getSupportActionBar().setTitle("Post Blood Request");
//        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        text1 = findViewById(R.id.getTitle);
        text2 = findViewById(R.id.getDate);
        text3 = findViewById(R.id.getCurrency);
        text4 = findViewById(R.id.getGenre);
        imageView = findViewById(R.id.imageView);
        text6 = findViewById(R.id.getTime);
        loc=findViewById(R.id.getLocation);

        btnpost = findViewById(R.id.postbtn);

        cal = Calendar.getInstance();

        int day = cal.get(Calendar.DAY_OF_MONTH);
        int month = cal.get(Calendar.MONTH);
        int year = cal.get(Calendar.YEAR);
        int hour = cal.get(Calendar.HOUR);
        int min = cal.get(Calendar.MINUTE);
        month+=1;
        Time = "";
        Date = "";
        String ampm="AM";

        if(cal.get(Calendar.AM_PM) ==1)
        {
            ampm = "PM";
        }

        if(hour<10)
        {
            Time += "0";
        }
        Time += hour;
        Time +=":";

        if(min<10) {
            Time += "0";
        }

        Time +=min;
        Time +=(" "+ampm);

        Date = day+"/"+month+"/"+year;

        ref=FirebaseDatabase.getInstance().getReference().child("events");
        sref= FirebaseStorage.getInstance().getReference().child("Poster");
        imageView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent();
                intent.setType("image/+");
                intent.setAction(Intent.ACTION_GET_CONTENT);
                startActivityForResult(intent,REQUEST_CODE_IMAGE);
            }
        });
        FirebaseUser cur_user = mAuth.getInstance().getCurrentUser();

        if(cur_user == null)
        {
            startActivity(new Intent(AddEventsScreen.this, LoginScreen.class));
        } else {
            uid = cur_user.getUid();
        }

        mAuth = FirebaseAuth.getInstance();
        fdb = FirebaseDatabase.getInstance();
        db_ref = fdb.getReference("events");

        try {
            btnpost.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    pd.show();
                    final Query findname = fdb.getReference("users").child(uid);
final  String Imagename = text1.getText().toString();
if(isImageAdded!=false&& Imagename!=null){
    uploadImage(Imagename);
}

                    if(text1.getText().length() == 0)
                    {
                        Toast.makeText(getApplicationContext(), "Enter Title!",
                                Toast.LENGTH_LONG).show();
                    }
                    else {
                        findname.addListenerForSingleValueEvent(new ValueEventListener() {
                            @Override
                            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {

                                if (dataSnapshot.exists()) {
                                    db_ref.child(uid).child("HostName").setValue(dataSnapshot.getValue(UserData.class).getUsername());
                                    db_ref.child(uid).child("Hoster").setValue(dataSnapshot.getValue(UserData.class));
                                    db_ref.child(uid).child("Title").setValue(text1.getText().toString());
                                    db_ref.child(uid).child("Date").setValue(text2.getText().toString());
                                    db_ref.child(uid).child("Currency").setValue(text3.getText().toString());
                                    db_ref.child(uid).child("Genre").setValue(text4.getText().toString());
                                    db_ref.child(uid).child("Poster").setValue(text5.getText().toString());
                                    db_ref.child(uid).child("Time").setValue(text6.getText().toString());
                                    db_ref.child(uid).child("Location").setValue(loc.getText().toString());

                                    Toast.makeText(AddEventsScreen.this, "Your post has been created successfully",
                                            Toast.LENGTH_LONG).show();
                                    startActivity(new Intent(AddEventsScreen.this, MainScreen.class));

                                } else {
                                    Toast.makeText(getApplicationContext(), "Database error occured.",
                                            Toast.LENGTH_LONG).show();
                                }

                            }

                            @Override
                            public void onCancelled(@NonNull DatabaseError databaseError) {
                                Log.d("User", databaseError.getMessage());

                            }
                        });
                    }
                }


            });
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
        pd.dismiss();

    }
    private void uploadImage(final String imagename) {
        final String key=ref.push().getKey();
        sref.child(key+".jpg").putFile(uri).addOnSuccessListener(new OnSuccessListener<UploadTask.TaskSnapshot>() {
            @Override
            public void onSuccess(UploadTask.TaskSnapshot taskSnapshot) {
                sref.child(key+"jpg").getDownloadUrl().addOnSuccessListener(new OnSuccessListener<Uri>() {
                    @Override
                    public void onSuccess(Uri uri) {
                        HashMap hashmap=new HashMap();
                        hashmap.put("Title",imagename);
                        hashmap.put("ImageUrl",uri.toString());

                        ref.child(key).setValue(hashmap).addOnSuccessListener(new OnSuccessListener<Void>() {
                            @Override
                            public void onSuccess(Void unused) {
                                Toast.makeText(AddEventsScreen.this, "Added", Toast.LENGTH_SHORT).show();
                            }
                        });
                    }
                });
            }
        }).addOnProgressListener(new OnProgressListener<UploadTask.TaskSnapshot>() {
            @Override
            public void onProgress(@NonNull UploadTask.TaskSnapshot snapshot) {

            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if(requestCode==REQUEST_CODE_IMAGE&& data!=null){
            uri=data.getData();
            isImageAdded=true;
            imageView.setImageURI(uri);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                onBackPressed();
                return true;
        }

        return super.onOptionsItemSelected(item);
    }
}