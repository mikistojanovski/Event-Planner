package com.example.diplomska3.ui.theme;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;

import androidx.appcompat.app.AppCompatActivity;

import com.example.diplomska3.R;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.FirebaseDatabase;

public class LoadingScreen extends AppCompatActivity{
    private FirebaseAuth mAuth;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_loading_screen);
        mAuth = FirebaseAuth.getInstance();

        if (true) {

        Handler handler = new Handler();
        handler.postDelayed(new Runnable() {
            @Override
            public void run() {
                startActivity(new Intent(LoadingScreen.this, LoginScreen.class));
                finish();

            }
        }, 2000);
        }
        else {
            Handler handler = new Handler();
            handler.postDelayed(new Runnable() {
                @Override
                public void run() {
                    startActivity(new Intent(LoadingScreen.this, MainScreen.class));
                    finish();

                }
            }, 2000);
        }

    }}
