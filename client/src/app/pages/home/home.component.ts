import { Component } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input'; 
import { FormsModule } from '@angular/forms'; 
import { RecipesComponent } from '../../../recipes/recipes.component';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ 
    FormsModule, 
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
  RecipesComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
