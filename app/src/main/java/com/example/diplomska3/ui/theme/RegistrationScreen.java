package com.example.diplomska3.ui.theme;

import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.example.diplomska3.R;
import com.example.diplomska3.viewmodels.UserData;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.auth.AuthResult;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.firebase.database.ValueEventListener;
public class RegistrationScreen extends AppCompatActivity {

    private EditText inputemail, inputpassword, retypePassword, userName;
    private FirebaseAuth mAuth;
    private Button btnSignup;
    private ProgressDialog pd;

    private boolean isUpdate = false;

    private DatabaseReference db_ref, guestref;
    private FirebaseDatabase db_User;
    private Spinner isDonor;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pd = new ProgressDialog(this);
        pd.setMessage("Loading...");
        pd.setCancelable(true);
        pd.setCanceledOnTouchOutside(false);
        pd.show();
        setContentView(R.layout.activity_registration_screen);

        db_User = FirebaseDatabase.getInstance();
        db_ref = db_User.getReference("users");
        guestref = db_User.getReference("guests");

        mAuth = FirebaseAuth.getInstance();

        inputemail = findViewById(R.id.input_userEmail);
        inputpassword = findViewById(R.id.input_password);
        retypePassword = findViewById(R.id.input_password_confirm);
        userName = findViewById(R.id.input_fullName);

        final String blood = isDonor.getSelectedItem().toString();


        btnSignup = findViewById(R.id.button_register);
//        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        if (mAuth.getCurrentUser() != null) {

            inputemail.setVisibility(View.GONE);
            inputpassword.setVisibility(View.GONE);
            retypePassword.setVisibility(View.GONE);
            btnSignup.setText("Update Profile");
            pd.dismiss();
            /// getActionBar().setTitle("Profile");
//            getSupportActionBar().setTitle("Profile");
            findViewById(R.id.image_logo).setVisibility(View.GONE);
            isUpdate = true;


            Query Profile = db_ref.child(mAuth.getCurrentUser().getUid());
            Profile.addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(@NonNull DataSnapshot dataSnapshot) {


                    UserData userData = dataSnapshot.getValue(UserData.class);

                    if (userData != null) {
                        pd.show();
                        userName.setText(userData.getUsername());
                        Query donor = db_ref.child("guest")
                                .child(mAuth.getCurrentUser().getUid());

                        donor.addListenerForSingleValueEvent(new ValueEventListener() {

                            @Override
                            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                                pd.dismiss();
                            }

                            @Override
                            public void onCancelled(@NonNull DatabaseError databaseError) {
                                Log.d("User", databaseError.getMessage());
                            }

                        });
                    }

                }

                @Override
                public void onCancelled(@NonNull DatabaseError databaseError) {
                    Log.d("User", databaseError.getMessage());
                }
            });


        } else pd.dismiss();

        btnSignup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                final String email = inputemail.getText().toString();
                final String password = inputpassword.getText().toString();
                final String ConfirmPassword = retypePassword.getText().toString();
                final String UserName = userName.getText().toString();
                final int roles = isDonor.getSelectedItemPosition();


                try {

                    if (UserName.length() <= 2) {
                        ShowError("UserName");
                        userName.requestFocusFromTouch();
                    } else {
                        if (!isUpdate) {
                            if (email.length() == 0) {
                                ShowError("Email ID");
                                inputemail.requestFocusFromTouch();
                            } else if (password.length() <= 5) {
                                ShowError("Password");
                                inputpassword.requestFocusFromTouch();
                            } else if (password.compareTo(ConfirmPassword) != 0) {
                                Toast.makeText(RegistrationScreen.this, "Password did not match!", Toast.LENGTH_LONG)
                                        .show();
                                retypePassword.requestFocusFromTouch();
                            } else {
                                pd.show();
                                mAuth.createUserWithEmailAndPassword(email, password)
                                        .addOnCompleteListener(RegistrationScreen.this, new OnCompleteListener<AuthResult>() {
                                            @Override
                                            public void onComplete(@NonNull Task<AuthResult> task) {

                                                if (!task.isSuccessful()) {
                                                    Toast.makeText(RegistrationScreen.this, "Registration failed! try agian.", Toast.LENGTH_LONG)
                                                            .show();
                                                    Log.v("error", task.getException().getMessage());
                                                } else {
                                                    UserData userData = new UserData(UserName,password,email,roles);
                                                    FirebaseDatabase.getInstance().getReference("users")
                                                            .child(FirebaseAuth.getInstance().getCurrentUser().getUid())
                                                            .setValue(userData).addOnCompleteListener(new OnCompleteListener<Void>() {
                                                                @Override
                                                                public void onComplete(@NonNull Task<Void> task) {
                                                                    if ((task.isSuccessful())) {
                                                                        Toast.makeText(getApplicationContext(), "Welcome, your account has been created!", Toast.LENGTH_LONG)
                                                                                .show();
                                                                    } else {
                                                                        Toast.makeText(getApplicationContext(), "Problem , try again!", Toast.LENGTH_LONG)
                                                                                .show();
                                                                    }
                                                                }

                                                            });

                                                    FirebaseDatabase.getInstance().getReference("guests")
                                                            .child(FirebaseAuth.getInstance().getCurrentUser().getUid())
                                                            .setValue(userData).addOnCompleteListener(new OnCompleteListener<Void>() {
                                                                @Override
                                                                public void onComplete(@NonNull Task<Void> task) {
                                                                    if ((task.isSuccessful())) {

                                                                        Intent intent = new Intent(RegistrationScreen.this, OpenAppScreen.class);

                                                                        startActivity(intent);

                                                                        finish();
                                                                    } else {
                                                                        Toast.makeText(getApplicationContext(), "Problem , try again!", Toast.LENGTH_LONG)
                                                                                .show();
                                                                    }
                                                                }

                                                            });
                                                }
                                                pd.dismiss();

                                            }

                                        });
                            }

                        } else {

                            String id = mAuth.getCurrentUser().getUid();
                            db_ref.child(id).child("UserName").setValue(UserName);
                            guestref.child(id).child("UserName").setValue(UserName);

                            if(isDonor.getSelectedItemPosition()>0)
                            {
                                guestref.child(blood).child(id).child("UID").setValue(id).toString();
                                guestref.child(blood).child(id).child("Name").setValue(UserName);

                            }
                            else
                            {

                                guestref.child(blood).child(id).removeValue();

                            }
                            Toast.makeText(getApplicationContext(), "Your account has been updated!", Toast.LENGTH_LONG)
                                    .show();
                            Intent intent = new Intent(RegistrationScreen.this, OpenAppScreen.class);
                            startActivity(intent);
                            finish();
                        }
                        pd.dismiss();

                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }

            }
        });
    }

    private void ShowError(String error) {

        Toast.makeText(RegistrationScreen.this, "Please, Enter a valid "+error,
                Toast.LENGTH_LONG).show();
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
