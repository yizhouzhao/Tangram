import numpy as np
from PIL import Image
from IPython.display import display

def point_line_symmetry(x_0, y_0, a, b, c):
    # x_1 = ((a**2 - b**2)*x_0 - 2*a*b*y_0 - 2*b*c)/(a**2 + b**2)
    # y_1 = ((b**2 - a**2)*y_0 - 2*a*b*x_0 - 2*a*c)/(a**2 + b**2)

    x_1 = ((b ** 2 - a ** 2) * x_0 - 2 * a * b * y_0 - 2 * a * c) / (a ** 2 + b ** 2)
    y_1 = ((a ** 2 - b ** 2) * y_0 - 2 * a * b * x_0 - 2 * b * c) / (a ** 2 + b ** 2)

    return x_1, y_1


def matrix_line_symmetry(matrix, a, b, c):
    up_sum = 0
    down_sum = 0

    assert len(matrix) > 0 and len(matrix[0]) > 0
    row = len(matrix)
    col = len(matrix[0])
    for i in range(row):
        for j in range(col):
            if matrix[i][j] > 0:
                point_sign = a * i + b * j + c
                if point_sign < 0:
                    down_sum += 1
                elif point_sign > 0:
                    up_sum += 1

    if up_sum < down_sum:
        fold_direction = 1
    else:
        fold_direction = -1

    new_matrix = np.zeros((row, col))
    for i in range(row):
        for j in range(col):
            if matrix[i][j] > 0:
                point_sign = a * i + b * j + c
                # print("point sign", i, j, point_sign)
                if fold_direction * point_sign > 0:
                    new_point = point_line_symmetry(i, j, a, b, c)
                    round_x = round(new_point[0])
                    round_y = round(new_point[1])
                    if round_x >= 0 and round_x < row and round_y >= 0 and round_y < col:
                        new_matrix[round_x][round_y] = 1.0
                else:
                    new_matrix[i][j] = 1.0

    return new_matrix


def read_cloth_image(cloth_file, show_image=True):
    im = Image.open(cloth_file).convert("L")
    if show_image:
        display(im)
    im_np = np.asarray(im)
    im_np = 1 - im_np / 255.0

    for i in range(len(im_np)):
        for j in range(len(im_np[0])):
            if im_np[i][j] > 0:
                im_np[i][j] = 1

    return im_np


def fold_action_min(cloth, value_model, transforms_after, method="greedy"):
    img = Image.fromarray(np.uint8(cloth * 255), 'L')
    img_t = transforms_after(img)
    img_t_score = value_model(img_t.unsqueeze(0)).item()

    action = (1, 0, 0)
    folded_cloth_save_image = None
    min_score = 1.0
    for i in range(1, 11):
        c = - i * 7
        for a, b in [(1, 0), (0, 1)]:
            cloth_folded = matrix_line_symmetry(cloth, a, b, c)
            img_folded = Image.fromarray(np.uint8(cloth_folded * 255), 'L')
            img_folded_t = transforms_after(img_folded)
            img_folded_t_score = value_model(img_folded_t.unsqueeze(0)).item()

            if min_score > img_folded_t_score:
                print(action, min_score)
                min_score = img_folded_t_score
                action = (a, b, c)
                folded_cloth_save_image = cloth_folded

    return action, min_score, folded_cloth_save_image

def fold_action_max(cloth, value_model, transforms_after, method="greedy", use_cuda = False):
    img = Image.fromarray(np.uint8(cloth * 255), 'L')
    img_t = transforms_after(img)
    # img_t_score = value_model(img_t.unsqueeze(0)).item()

    action = (1, 0, 0)
    folded_cloth_save_image = None
    max_score = -1

    if np.random.rand() < 0.2:
        if np.random.randint(2)> 0:
            a = 1
            b = 0
        else:
            a = 0
            b = 1

        c = np.random.randint(10) + 1
        cloth_folded = matrix_line_symmetry(cloth, a, b, c)
        action = (a, b, c)
        return action, 0, cloth_folded

    for i in range(1, 11):
        c = - i * 7
        for a, b in [(1, 0), (0, 1)]:
            cloth_folded = matrix_line_symmetry(cloth, a, b, c)
            img_folded = Image.fromarray(np.uint8(cloth_folded * 255), 'L')
            img_folded_t = transforms_after(img_folded)
            if use_cuda:
                img_folded_t = img_folded_t.to("cuda")
            img_folded_t_score = value_model(img_folded_t.unsqueeze(0)).item()

            if max_score < img_folded_t_score:
                # print(action, max_score)
                max_score = img_folded_t_score
                action = (a, b, c)
                folded_cloth_save_image = cloth_folded

    return action, max_score, folded_cloth_save_image


def calculate_image_score(cloth_img: np.array, transform_img, model_img, use_cuda=False):
    img = Image.fromarray(np.uint8(cloth_img * 255), 'L')
    img_t = transform_img(img)
    if use_cuda:
        img_t = img_t.to("cuda")
    img_t_score = model_img(img_t.unsqueeze(0))

    return img_t_score



def fold_action_max_beam_search(cloth, value_model, transforms_after, steps = 2, use_cuda = False):
    folded_cloth_save_image = []
    max_score = -1
    max_index = -1

    #init
    future_cloth_list = []
    future_cloth_list.append([cloth])

    for step in range(steps):
        next_cloth_list = []
        for temp_cloth in future_cloth_list[-1]:
            for i in range(1, 11):
                c = - i * 7
                for a, b in [(1, 0), (0, 1)]:
                    cloth_folded = matrix_line_symmetry(temp_cloth, a, b, c)
                    next_cloth_list.append(cloth_folded)

        future_cloth_list.append(next_cloth_list)

    for i, temp_cloth in enumerate(future_cloth_list[-1]):
        img_folded = Image.fromarray(np.uint8(temp_cloth * 255), 'L')
        img_folded_t = transforms_after(img_folded)
        if use_cuda:
            img_folded_t = img_folded_t.to("cuda")
        img_folded_t_score = value_model(img_folded_t.unsqueeze(0)).item()

        if max_score < img_folded_t_score:
            # print(action, max_score)
            max_score = img_folded_t_score
            max_index = i

    for i in range(len(future_cloth_list)-1, 0, -1):
        folded_cloth_save_image.append(future_cloth_list[i][max_index])
        max_index = max_index // 20

    return max_score, folded_cloth_save_image