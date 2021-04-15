import bpy
from random import *



# genetic algorithm
def genetic_algorithm(auto, nbJ, nbA, nbB, n_bits, n_iter, n_pop, r_cross, r_mut):
    pop = chromosomeAleatoire(n_pop)
    notes = []
    best_eval = 0
    best = [0, 0, 0,  0, 0, 0]
    # enumerate generations
    for gen in range(n_iter):
        # evaluate all candidates in the population
        print(">%d, best f(%s) = %.3f" % (gen,  best, best_eval))
        if(auto):
            for i in range(n_pop):
#                StringBitBras = str(pop[i][mode*2]) + str(pop[i][mode*2+1])
#                nbBitBras = int(StringBitBras, 2)
                StringBitBras = str(pop[i][0]) + str(pop[i][1])
                nbBitJambes = int(StringBitBras, 2)
                StringBitBras = str(pop[i][2]) + str(pop[i][3])
                nbBitAiles = int(StringBitBras, 2)
                StringBitBras = str(pop[i][4]) + str(pop[i][5])
                nbBitBras = int(StringBitBras, 2)
                if (nbBitJambes != nbJ):
                    nbBitJambes = 0
                if (nbBitAiles != nbA):
                    nbBitAiles = 0
                if (nbBitBras != nbB):
                    nbBitBras = 0
                notes.append(nbBitJambes * 10 + nbBitAiles * 10 + nbBitBras * 10)
        else:
            for i in range(n_pop):
                GenerationCreature(pop[i], (0, 0, 0))
                bpy.ops.wm.redraw_timer(type='DRAW_WIN_SWAP', iterations=1)
                print("note")
                notes.append(input())
                supprimerScene()
            
        # check for new best solution
        for i in range(n_pop):
            if int(notes[i]) > best_eval:
                best, best_eval = pop[i], int(notes[i])
                print(">%d, new best f(%s) = %.3f" % (gen,  pop[i], int(notes[i])))
                
        # select parents
        selected = [selection(pop, notes) for _ in range(n_pop)]
        
        bests = [bests for _,bests in sorted(zip(notes,pop))]
        
        keep = bests[-6:]
        
        # create the next generation
        children = []
        for i in range(0, n_pop, 2):
            
            # get selected parents in pairs
            p1, p2 = selected[i], selected[i+1]
            
            # crossover and mutation
            for c in crossover(p1, p2, r_cross):
                
                # mutation
                mutation(c, r_mut)
                
                # store for next generation
                children.append(c)
                
        # replace population
        pop = children
#        pop = children[0:19]
#        pop.append(keep)
#        print(pop)
    return [pop, best, best_eval]


# tournament selection
def selection(pop, scores, k=3):
    # first random selection
    selection_ix = randint(0, len(pop) - 1)
    for ix in range(0, len(pop), k-1):
        # check if better (e.g. perform a tournament)
        if scores[ix] > scores[selection_ix]:
            selection_ix = ix
    return pop[selection_ix]

# crossover two parents to create two children
def crossover(p1, p2, r_cross):
    # children are copies of parents by default
    c1, c2 = p1.copy(), p2.copy()
    # check for recombination
    if randint(0, 1) < r_cross:
        # select crossover point that is not on the end of the string
        pt = randint(1, len(p1)-2)
        # perform crossover
        c1 = p1[:pt] + p2[pt:]
        c2 = p2[:pt] + p1[pt:]
    return [c1, c2]

# mutation operator
def mutation(bitstring, r_mut):
    for i in range(len(bitstring)):
        # check for a mutation
        if uniform(0, 1) < r_mut:
            # flip the bit
            bitstring[i] = 1 - bitstring[i]


def supprimerScene():
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False, confirm=False)
    
#***************** FONCTION chromosomeAleatoire ***************************

def chromosomeAleatoire(n_pop):
    tabChromosome = [[0] * 6 for _ in range(n_pop)]
    
    for i in range(n_pop):
        for j in range(6):
            tabChromosome[i][j] = randint(0,1)
    return tabChromosome

#***************** FONCTION GenerationCreature ***************************

indexNom = 0

def GenerationCreature(chromosome, pose):
    
    global indexNom
    print(chromosome)

    bpy.ops.mesh.primitive_cube_add(location = pose)
    
    mat = bpy.data.materials.new("PKHG")
    mat.diffuse_color = (uniform(0,1),uniform(0,1),uniform(0,1),1)
    o = bpy.context.selected_objects[0] 
    o.active_material = mat

    bpy.ops.object.modifier_add(type='MIRROR')
    bpy.context.object.modifiers["Mirror"].use_axis[0] = False
    bpy.context.object.modifiers["Mirror"].use_axis[1] = True
    obj = bpy.context.active_object

        
    StringBitJambes = str(chromosome[0]) + str(chromosome[1])
    nbBitJambes = int(StringBitJambes, 2)

    # Sélection d'une face JAMBES -------------------
    jambesZ = uniform(3, 5)
    #nbJambes = randint(0, 3)
    print('jambes : ', nbBitJambes)
    for i in range(nbBitJambes):
        jambesY = uniform(1, 3)
        jambesX = uniform(1, 3)
        bpy.ops.object.mode_set(mode = 'OBJECT')
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.mode_set(mode = 'EDIT') 
        bpy.ops.mesh.select_mode(type="FACE")
        bpy.ops.mesh.select_all(action = 'DESELECT')
        bpy.ops.object.mode_set(mode = 'OBJECT')
        obj.data.polygons[0].select = True
        bpy.ops.object.mode_set(mode = 'EDIT') 

        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(jambesX, jambesY, jambesZ)})
        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(-jambesX, jambesY, jambesZ)})

    StringBitAiles = str(chromosome[2]) + str(chromosome[3])
    nbBitAiles = int(StringBitAiles, 2)

    #Gestion des ailes
    left = (uniform(2, 3),3, -uniform(2,4))
    right = (uniform(2, 3), 3, -uniform(2, 4))
    #val = randint(1,3)
    print('ailes : ', nbBitAiles)
    for i in range(nbBitAiles):
        bpy.ops.object.mode_set(mode = 'OBJECT')
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.mode_set(mode = 'EDIT') 
        bpy.ops.mesh.select_mode(type="FACE")
        bpy.ops.mesh.select_all(action = 'DESELECT')
        bpy.ops.object.mode_set(mode = 'OBJECT')
        obj.data.polygons[2].select = True
        bpy.ops.object.mode_set(mode = 'EDIT') 
        left = (left[0]+i,left[1],left[2])
        right = (right[0]+i,right[1],right[2])
        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value": left})


        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":right})



    # Sélection d'une face --------------------------------BRAS
    StringBitBras = str(chromosome[4]) + str(chromosome[5])
    nbBitBras = int(StringBitBras, 2)

    brasY = uniform(2, 3)
    #nbBras = randint(0, 3)
    print('bras : ', nbBitBras)
    for i in range(nbBitBras):
        brasZ = uniform(1, 3)
        brasX = uniform(-2, 2)
        bpy.ops.object.mode_set(mode = 'OBJECT')
        obj = bpy.context.active_object
        bpy.ops.object.mode_set(mode = 'EDIT') 
        bpy.ops.mesh.select_mode(type="FACE")
        bpy.ops.mesh.select_all(action = 'DESELECT')
        bpy.ops.object.mode_set(mode = 'OBJECT')
        obj.data.polygons[4].select = True
        bpy.ops.object.mode_set(mode = 'EDIT') 
        
        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(brasX, brasY, brasZ)})
        bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(-brasX, brasY, brasZ)})


    # Sélection d'une face
    bpy.ops.object.mode_set(mode = 'OBJECT')
    obj = bpy.context.active_object
    bpy.ops.object.mode_set(mode = 'EDIT')
    bpy.ops.mesh.select_mode(type="FACE")
    bpy.ops.mesh.select_all(action = 'DESELECT')
    bpy.ops.object.mode_set(mode = 'OBJECT')
    obj.data.polygons[3].select = True
    bpy.ops.object.mode_set(mode = 'EDIT') 

    bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(-uniform(-3, 3), -uniform(-3, 3), -uniform(-3, 3))})
    bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(uniform(-2, 3), uniform(-2, 2), uniform(-3, 3))})



    # Sélection d'une face
    bpy.ops.object.mode_set(mode = 'OBJECT')
    obj = bpy.context.active_object
    bpy.ops.object.mode_set(mode = 'EDIT') 
    bpy.ops.mesh.select_mode(type="FACE")
    bpy.ops.mesh.select_all(action = 'DESELECT')
    bpy.ops.object.mode_set(mode = 'OBJECT')
    obj.data.polygons[6].select = True
    bpy.ops.object.mode_set(mode = 'EDIT') 

    bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value":(uniform(-2, 3), uniform(-2, 2), uniform(-4, 4))})


    bpy.ops.object.modifier_add(type='SUBSURF')
    bpy.context.object.modifiers["Subdivision"].levels = 3
    bpy.ops.object.mode_set(mode = 'OBJECT')
    
    chemin = "C:/Users/tasha/Desktop/AP & IA/Spores/Assets/Prefab/Creatures/Creaturestruc" + str(indexNom) + ".obj"
    indexNom += 1
#    bpy.ops.export_scene.obj(filepath=chemin)
    
    return chromosome

supprimerScene()
tab = genetic_algorithm(True, 2, 2, 3, 6, 5, 26, 0.4, 0.1)[0]
for i in range(6):
    for j in range(4):
#        supprimerScene()
#        GenerationCreature(tab[i*j],(0, 0, 0))
        GenerationCreature(tab[i*j],(0,i*20,j*20))
#GenerationCreature([1, 1, 1, 1, 1, 1])