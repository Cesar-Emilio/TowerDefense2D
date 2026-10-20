"""Genera todo el pixel art del Tower Defense de Dia de Muertos.

Cada sprite se dibuja con figuras simples (elipses, rectangulos, triangulos) sobre una cuadricula,
con una paleta compartida y un contorno oscuro automatico. Las animaciones son cuadros numerados
(Nombre_0.png, Nombre_1.png...). Uso:  py -I generate_sprites.py <carpeta Assets/Art>
"""
import math
import os
import sys

from PIL import Image

# ───────────────────────────── Paleta ─────────────────────────────
K = (26, 16, 32)            # contorno
W = (244, 239, 230); w = (205, 196, 214)                 # hueso
P = (98, 46, 140); p = (60, 26, 92)                      # morado
O = (255, 154, 31); o = (214, 96, 12)                    # cempasuchil
Y = (255, 214, 64); F = (255, 244, 180)                  # amarillo / flama
R = (232, 51, 109); r = (160, 30, 76)                    # rosa
G = (70, 176, 76); g = (36, 112, 46)                     # verde
B = (64, 182, 232); b = (34, 110, 170)                   # azul
T = (40, 196, 170); t = (20, 124, 112)                   # turquesa
D = (58, 58, 76); d = (32, 32, 44); L = (112, 112, 134)  # metal
S = (205, 210, 222); s = (140, 146, 164)                 # plata
X = (36, 30, 44); x = (62, 52, 72)                       # negro tela
N = (122, 74, 42); n = (80, 46, 24)                      # madera
C = (184, 92, 52); c = (130, 60, 34)                     # barro
M = (190, 34, 44)                                        # rojo
Q = (222, 206, 250); q = (168, 150, 214)                 # lavanda
H = (72, 30, 96); h = (108, 52, 132)                     # cabello
GH = {"white": (W, w), "blue": ((140, 190, 255), (84, 128, 214)),
      "green": ((150, 236, 150), (76, 170, 96)), "purple": ((200, 150, 255), (128, 80, 200))}


class Canvas:
    def __init__(self, size):
        self.size = size
        self.px = [[None] * size for _ in range(size)]

    def set(self, xx, yy, col):
        xx, yy = int(xx), int(yy)
        if 0 <= xx < self.size and 0 <= yy < self.size:
            self.px[yy][xx] = col

    def get(self, xx, yy):
        if 0 <= xx < self.size and 0 <= yy < self.size:
            return self.px[yy][xx]
        return None

    def ellipse(self, cx, cy, rx, ry, col, only=None):
        for yy in range(self.size):
            for xx in range(self.size):
                dx = (xx + 0.5 - cx) / rx
                dy = (yy + 0.5 - cy) / ry
                if dx * dx + dy * dy <= 1.0 and (only is None or self.px[yy][xx] in only):
                    self.px[yy][xx] = col

    def circle(self, cx, cy, rr, col, only=None):
        self.ellipse(cx, cy, rr, rr, col, only)

    def rect(self, x0, y0, x1, y1, col, only=None):
        for yy in range(int(y0), int(y1) + 1):
            for xx in range(int(x0), int(x1) + 1):
                if only is None or self.get(xx, yy) in only:
                    self.set(xx, yy, col)

    def tri(self, a, b2, c2, col):
        xs = [a[0], b2[0], c2[0]]
        ys = [a[1], b2[1], c2[1]]
        for yy in range(int(min(ys)), int(max(ys)) + 1):
            for xx in range(int(min(xs)), int(max(xs)) + 1):
                px_, py_ = xx + 0.5, yy + 0.5
                d1 = (px_ - b2[0]) * (a[1] - b2[1]) - (a[0] - b2[0]) * (py_ - b2[1])
                d2 = (px_ - c2[0]) * (b2[1] - c2[1]) - (b2[0] - c2[0]) * (py_ - c2[1])
                d3 = (px_ - a[0]) * (c2[1] - a[1]) - (c2[0] - a[0]) * (py_ - a[1])
                neg = d1 < 0 or d2 < 0 or d3 < 0
                pos = d1 > 0 or d2 > 0 or d3 > 0
                if not (neg and pos):
                    self.set(xx, yy, col)

    def line(self, x0, y0, x1, y1, col, width=1.0):
        steps = int(max(abs(x1 - x0), abs(y1 - y0)) * 2) + 1
        for i in range(steps + 1):
            k = i / steps
            px_ = x0 + (x1 - x0) * k
            py_ = y0 + (y1 - y0) * k
            if width <= 1.0:
                self.set(px_, py_, col)
            else:
                self.circle(px_ + 0.5, py_ + 0.5, width / 2, col)

    def pixels(self, pts, col):
        for xx, yy in pts:
            self.set(xx, yy, col)

    def outline(self, col=K):
        filled = [[self.px[yy][xx] is not None for xx in range(self.size)] for yy in range(self.size)]
        for yy in range(self.size):
            for xx in range(self.size):
                if filled[yy][xx]:
                    continue
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx_, ny_ = xx + dx, yy + dy
                    if 0 <= nx_ < self.size and 0 <= ny_ < self.size and filled[ny_][nx_]:
                        self.px[yy][xx] = col
                        break

    def save(self, path):
        img = Image.new("RGBA", (self.size, self.size), (0, 0, 0, 0))
        for yy in range(self.size):
            for xx in range(self.size):
                col = self.px[yy][xx]
                if col is not None:
                    img.putpixel((xx, yy), col if len(col) == 4 else col + (255,))
        os.makedirs(os.path.dirname(path), exist_ok=True)
        img.save(path)


def h2(xx, yy, seed=0):
    """Ruido determinista: el mismo pixel siempre da el mismo valor (hace los tiles repetibles)."""
    v = (xx * 374761393 + yy * 668265263 + seed * 2147483647) & 0xFFFFFFFF
    v = (v ^ (v >> 13)) * 1274126177 & 0xFFFFFFFF
    return (v ^ (v >> 16)) / 0xFFFFFFFF


# ───────────────────────────── Calaveras ─────────────────────────────
def skull_face(cv, cx, cy, eye_ring, cheek, glow=False):
    cv.ellipse(cx, cy, 7.5, 8, W)
    cv.rect(int(cx - 5), int(cy + 4), int(cx + 4), int(cy + 8), W)
    cv.ellipse(cx + 3.5, cy + 1, 3.5, 7, w)
    cv.ellipse(cx - 0.5, cy, 6.5, 7.5, W)
    for ex in (cx - 3.5, cx + 3.5):
        cv.circle(ex, cy - 0.5, 3, eye_ring)
        cv.circle(ex, cy - 0.5, 2, K)
        cv.set(int(ex) - 1, int(cy - 0.5) - 1, Y if glow else W)
    cv.pixels([(int(cx) - 1, int(cy + 3)), (int(cx), int(cy + 3)), (int(cx) - 1, int(cy + 4))], K)
    my = int(cy + 6)
    for xx in range(int(cx) - 4, int(cx) + 4):
        cv.set(xx, my, K)
    for xx in range(int(cx) - 4, int(cx) + 4, 2):
        cv.set(xx, my - 1, K)
        cv.set(xx, my + 1, K)
    cv.set(int(cx) - 6, int(cy + 3), cheek)
    cv.set(int(cx) + 5, int(cy + 3), cheek)


def flower(cv, cx, cy, petal, center):
    cv.circle(cx, cy, 2.2, petal)
    cv.set(int(cx), int(cy), center)


# ───────────────────────────── Torres ─────────────────────────────
def catrina(f):
    cv = Canvas(32)
    skull_face(cv, 16, 19.5, R, R, glow=f == 1)
    cv.pixels([(15, 13), (16, 13), (15, 14), (16, 14)], Y)
    cv.ellipse(16, 6, 6.5, 4.5, P)
    cv.rect(10, 6, 21, 10, P)
    cv.rect(10, 9, 21, 9, R)
    cv.ellipse(14, 4.5, 2.5, 1.5, p)
    cv.ellipse(16, 11.5, 15.5, 3, P)
    cv.ellipse(16, 12.5, 13, 1.6, p)
    flower(cv, 10.5, 8.5, O if f == 0 else Y, Y if f == 0 else O)
    flower(cv, 21.5, 8.5, O if f == 0 else Y, Y if f == 0 else O)
    flower(cv, 16, 7.5, R, Y)
    cv.pixels([(13, 10), (19, 10)], G)
    cv.outline()
    return cv


def calaverita(f):
    cv = Canvas(32)
    for i in range(10):
        cv.circle(15 + i * 0.9, 12 - i * 1.0, 3.3 - i * 0.08, D)
    for i in range(9):
        cv.circle(13.8 + i * 0.9, 11.2 - i * 1.0, 1.0, L)
    cv.circle(23.6, 2.6, 3.1, d)
    cv.circle(23.8, 2.4, 1.6, K)
    cv.circle(18.5, 8.2, 3.6, o)
    cv.circle(18.5, 8.2, 2.9, D)
    cv.circle(17.2, 8.8, 1.0, L)
    cv.circle(13.5, 13.5, 4.2, D)
    cv.circle(12.5, 12.5, 1.4, L)
    skull_face(cv, 15, 21, O, O)
    cv.pixels([(11, 15), (19, 15)], O)
    cv.pixels([(13, 14), (17, 14)], B)
    cv.pixels([(15, 14)], R)
    cv.outline()
    # mecha con chispa que parpadea
    cv.pixels([(9, 11), (8, 10)], n)
    cv.pixels([(7, 9)] if f == 0 else [(7, 8), (6, 9)], Y)
    cv.pixels([(6, 8)] if f == 0 else [(8, 8)], F)
    return cv


def cempasuchil(f):
    cv = Canvas(32)
    cv.ellipse(7, 26, 6, 2.5, G)
    cv.ellipse(25, 26, 6, 2.5, G)
    cv.ellipse(7, 27, 5, 1.2, g)
    cv.ellipse(25, 27, 5, 1.2, g)
    cx, cy = 16, 15
    turn = 0.26 if f == 1 else 0.0
    for i in range(12):
        a = i * math.tau / 12 + turn
        cv.circle(cx + math.cos(a) * 10.5, cy + math.sin(a) * 10.5, 3.2, o)
    cv.circle(cx, cy, 11, o)
    for i in range(10):
        a = i * math.tau / 10 + 0.3 - turn
        cv.circle(cx + math.cos(a) * 8, cy + math.sin(a) * 8, 2.8, O)
    cv.circle(cx, cy, 8, O)
    for i in range(8):
        a = i * math.tau / 8 + turn
        cv.circle(cx + math.cos(a) * 5, cy + math.sin(a) * 5, 2.2, Y)
    cv.circle(cx, cy, 4.5, Y)
    cv.circle(cx, cy, 2.2, o)
    cv.pixels([(15, 14), (17, 16)], Y)
    cv.outline()
    return cv


def xolo(f):
    cv = Canvas(32)
    dog, dog2 = (70, 62, 78), (48, 42, 56)
    # cola (se mueve)
    if f == 0:
        cv.line(22, 25, 27, 21, dog, 2)
    else:
        cv.line(22, 25, 28, 25, dog, 2)
    cv.ellipse(16, 22, 7, 7, dog)
    cv.ellipse(18, 23, 4, 6, dog2)
    cv.rect(11, 25, 13, 29, dog)
    cv.rect(18, 25, 20, 29, dog)
    cv.rect(11, 29, 13, 29, x)
    cv.rect(18, 29, 20, 29, x)
    # orejas grandes (una se mueve)
    cv.tri((9, 10), (8, 1 if f == 0 else 2), (13, 8), dog)
    cv.tri((23, 10), (24, 1), (19, 8), dog)
    cv.tri((10, 8), (9, 4 if f == 0 else 5), (12, 8), R)
    cv.tri((22, 8), (23, 4), (20, 8), R)
    cv.ellipse(16, 12, 6.5, 5.5, dog)
    cv.ellipse(16, 15.5, 3.2, 2.5, (96, 86, 104))
    cv.pixels([(15, 14), (16, 14)], K)
    cv.pixels([(13, 11), (19, 11)], Y)
    cv.pixels([(13, 12), (19, 12)], K)
    cv.pixels([(15, 7), (16, 6), (17, 7), (16, 8)], (180, 180, 196))   # copete
    cv.rect(11, 17, 21, 18, R)                                          # collar
    cv.circle(16, 19.5, 1.6, O)
    cv.outline()
    return cv


def alebrije(f):
    cv = Canvas(32)
    # alas (arriba / abajo)
    if f == 0:
        cv.tri((10, 17), (1, 6), (5, 20), P)
        cv.tri((22, 17), (31, 6), (27, 20), P)
        cv.tri((8, 15), (3, 8), (6, 17), O)
        cv.tri((24, 15), (29, 8), (26, 17), O)
    else:
        cv.tri((10, 17), (1, 25), (6, 26), P)
        cv.tri((22, 17), (31, 25), (26, 26), P)
        cv.tri((8, 20), (3, 24), (6, 24), O)
        cv.tri((24, 20), (29, 24), (26, 24), O)
    cv.line(22, 25, 29, 28, R, 2)                       # cola
    cv.pixels([(29, 27), (30, 28)], Y)
    cv.ellipse(16, 21, 7, 6, T)
    cv.rect(12, 25, 13, 29, T)
    cv.rect(19, 25, 20, 29, T)
    for yy in (18, 21, 24):                             # rayas
        cv.rect(11, yy, 21, yy, R, only=[T])
    cv.pixels([(13, 19), (19, 20), (15, 23), (18, 23), (12, 22)], Y)
    cv.ellipse(16, 11, 5.5, 4.5, T)
    cv.ellipse(16, 14, 3, 2, t)
    cv.tri((12, 8), (10, 3), (14, 7), O)                # cuernos
    cv.tri((20, 8), (22, 3), (18, 7), O)
    cv.pixels([(13, 10), (19, 10)], Y)
    cv.pixels([(13, 11), (19, 11)], K)
    cv.pixels([(14, 14), (16, 15), (18, 14)], W)        # colmillos
    cv.pixels([(16, 8), (15, 9), (17, 9)], R)
    cv.outline()
    return cv


def ofrenda(f):
    cv = Canvas(32)
    cv.rect(3, 21, 28, 27, P)
    cv.rect(3, 21, 28, 22, O)
    for xx in range(3, 29, 3):                          # papel picado
        cv.set(xx, 23, R)
        cv.set(xx + 1, 23, Y)
    cv.rect(4, 28, 5, 30, n)
    cv.rect(26, 28, 27, 30, n)
    cv.rect(7, 15, 24, 20, O)
    cv.rect(7, 15, 24, 15, Y)
    cv.rect(11, 10, 20, 14, W)
    cv.rect(13, 3, 18, 9, N)                            # retrato
    cv.rect(14, 4, 17, 8, w)
    cv.ellipse(15.5, 6, 1.4, 1.6, x)
    for cxx in (5, 26):                                 # veladoras grandes
        cv.rect(cxx - 1, 14, cxx, 20, W)
        cv.rect(cxx - 1, 14, cxx, 14, w)
    for cxx in (9, 22):                                 # veladoras chicas
        cv.rect(cxx, 11, cxx + 1, 14, W)
    cv.ellipse(10, 19.5, 2.5, 1.6, (200, 140, 70))      # pan de muerto
    cv.pixels([(9, 19), (11, 19)], (150, 96, 44))
    cv.ellipse(21, 19.5, 2, 1.6, W)                     # calaverita de azucar
    cv.pixels([(20, 19), (22, 19)], R)
    for xx in (8, 12, 19, 23):
        cv.set(xx, 15, R)
    cv.outline()
    flames = [(5, 13), (26, 13), (9, 10), (22, 10)] if f == 0 else [(4, 13), (26, 12), (10, 10), (22, 9)]
    for fx, fy in flames:
        cv.set(fx, fy, Y)
        cv.set(fx, fy - 1, F)
    return cv


# ───────────────────────────── Enemigos ─────────────────────────────
def ghost(kind, f):
    body, shade = GH[kind]
    cv = Canvas(32)
    bob = f
    cv.circle(16, 13 + bob, 9, body)
    for xx in range(7, 25):
        extra = 2 if ((xx + f * 2) // 3) % 2 == 0 else 0
        cv.rect(xx, 13 + bob, xx, 24 + bob + extra, body)
    cv.ellipse(6.5, 18 + bob, 2.5, 1.6, body)
    cv.ellipse(25.5, 17 + bob - f, 2.5, 1.6, body)
    for yy in range(32):
        for xx in range(20, 26):
            if cv.get(xx, yy) == body and h2(xx, yy) > 0.35:
                cv.set(xx, yy, shade)
    cv.ellipse(12.5, 13 + bob, 1.6, 2.2, K)
    cv.ellipse(19.5, 13 + bob, 1.6, 2.2, K)
    cv.pixels([(12, 12 + bob), (19, 12 + bob)], W)
    cv.ellipse(16, 18.5 + bob, 1.5, 1.8 + f * 0.4, K)
    cv.pixels([(10, 16 + bob), (22, 16 + bob)], R)
    cv.outline()
    return cv


def pumpkin(f, small=False):
    cv = Canvas(32)
    k = 0.7 if small else 1.0
    cx, cy = 16, 18 if not small else 20
    cv.ellipse(cx, cy, 12 * k, 10 * k, o)
    cv.ellipse(cx - 6 * k, cy, 4.6 * k, 9.4 * k, O)
    cv.ellipse(cx + 6 * k, cy, 4.6 * k, 9.4 * k, O)
    cv.ellipse(cx, cy, 5 * k, 10 * k, O)
    cv.ellipse(cx - 7 * k, cy - 4 * k, 1.5 * k, 2.5 * k, Y)   # brillo
    cv.rect(cx - 1, cy - 12 * k, cx + 1, cy - 9 * k, g)        # tallo
    cv.ellipse(cx + 3 * k, cy - 10.5 * k, 2.5 * k, 1.2 * k, G)
    glow = Y if f == 0 else F
    ey = cy - 3 * k
    for ex in (cx - 5 * k, cx + 4 * k):
        cv.tri((ex - 2.2 * k, ey + 1.5 * k), (ex + 2.4 * k, ey + 1.5 * k), (ex + 0.1, ey - 2.2 * k), glow)
    cv.tri((cx - 1.2, cy + 1 * k), (cx + 1.2, cy + 1 * k), (cx, cy - 0.8 * k), glow)
    my = cy + 4 * k
    cv.rect(cx - 7 * k, my, cx + 7 * k, my + 2 * k, glow)
    for xx in range(int(cx - 6 * k), int(cx + 7 * k), 3):
        cv.set(xx, int(my), o)
        cv.set(xx + 1, int(my + 2 * k), o)
    cv.outline()
    return cv


def skeleton(f, warrior=False):
    cv = Canvas(32)
    off = 1 if f == 1 else 0
    # piernas
    if f == 0:
        cv.line(14, 23, 11, 30, W, 2)
        cv.line(18, 23, 21, 30, W, 2)
    else:
        cv.line(14, 23, 15, 30, W, 2)
        cv.line(18, 23, 17, 30, W, 2)
    cv.rect(13, 21, 19, 23, W)                       # pelvis
    cv.rect(15, 13, 16, 22, W)                       # columna
    for yy in (15, 17, 19):                          # costillas
        cv.rect(12, yy, 19, yy, W)
    # brazos (se balancean)
    cv.line(12, 14, 9, 21 - off, W, 2)
    cv.line(19, 14, 22, 21 + off, W, 2)
    cv.circle(16, 8, 5.2, W)
    cv.rect(13, 11, 18, 13, W)
    cv.rect(14, 7, 15, 9, K)
    cv.rect(17, 7, 18, 9, K)
    cv.pixels([(16, 10)], K)
    cv.pixels([(14, 12), (16, 12), (18, 12)], K)
    if warrior:
        cv.ellipse(16, 5.5, 6.2, 3.8, D)                # casco
        cv.ellipse(14, 4.5, 2.5, 1.2, L)
        cv.rect(15, 0, 17, 2, M)                        # penacho
        cv.ellipse(8.5, 19, 4, 5, N)                    # escudo
        cv.ellipse(8.5, 19, 2.6, 3.6, n)
        cv.circle(8.5, 19, 1.2, S)
        cv.line(23, 10 + off, 23, 23 + off, S, 1)       # espada
        cv.rect(21, 20 + off, 25, 20 + off, n)
    cv.outline()
    return cv


def banshee(f):
    cv = Canvas(32)
    bob = f
    # cabello largo
    cv.ellipse(16, 12 + bob, 9.5, 9, H)
    for xx in range(7, 26):
        bottom = 26 + bob + (1 if ((xx + f) // 2) % 2 == 0 else -1)
        cv.rect(xx, 12 + bob, xx, bottom, H if xx % 3 else h)
    # vestido
    cv.tri((16, 15 + bob), (9, 30), (23, 30), Q)
    for xx in range(9, 24, 2):
        cv.set(xx, 30, None)
        cv.set(xx + (f % 2), 29, q)
    # brazos al aire
    cv.line(12, 18 + bob, 5, 12 + bob - f, Q, 2)
    cv.line(20, 18 + bob, 27, 12 + bob - f, Q, 2)
    cv.ellipse(16, 12 + bob, 5, 6, Q)
    cv.ellipse(18, 13 + bob, 2.5, 5, q, only=[Q])
    cv.ellipse(14, 11 + bob, 1.1, 1.6, K)
    cv.ellipse(18, 11 + bob, 1.1, 1.6, K)
    cv.ellipse(16, 15.5 + bob, 1.4, 1.8 + f * 0.5, K)
    cv.outline()
    return cv


def charro(f):
    cv = Canvas(48)
    sway = f
    # cuerpo y chaqueta
    cv.tri((24, 29), (11, 47), (37, 47), X)
    cv.rect(16, 30, 32, 46, X)
    cv.rect(16, 46, 32, 47, x if f else X)
    cv.line(17, 32, 10, 43 + sway, X, 3)            # brazos
    cv.line(31, 32, 38, 43 - sway, X, 3)
    cv.circle(10, 44 + sway, 1.6, W)
    cv.circle(38, 44 - sway, 1.6, W)
    for yy in range(33, 46, 3):                     # botonadura de plata
        cv.set(20, yy, S)
        cv.set(28, yy, S)
    cv.rect(19, 38, 29, 39, M)                      # faja roja
    cv.tri((21, 30), (24, 33), (27, 30), M)         # mono
    # calavera
    cv.ellipse(24, 24, 7, 7.5, W)
    cv.rect(19, 27, 28, 31, W)
    cv.ellipse(27, 25, 3, 6, w, only=[W])
    eye = M if f == 0 else R
    cv.circle(21, 23, 2.4, K)
    cv.circle(27, 23, 2.4, K)
    cv.pixels([(21, 23), (27, 23)], eye)
    cv.pixels([(24, 26), (23, 27)], K)
    for xx in range(20, 29):
        cv.set(xx, 29, K)
    for xx in range(20, 29, 2):
        cv.set(xx, 28, K)
        cv.set(xx, 30, K)
    # sombrero enorme
    cv.ellipse(24, 15, 22.5, 5.2, X)
    cv.ellipse(24, 16.5, 19, 2.2, x)
    cv.ellipse(24, 8, 8, 7, X)
    cv.rect(16, 8, 32, 14, X)
    cv.rect(16, 12, 32, 13, S)
    for xx in range(17, 32, 3):
        cv.set(xx, 12, s)
    for i in range(0, 360, 12):                     # adorno plata del ala
        a = math.radians(i)
        cv.set(24 + math.cos(a) * 21, 15 + math.sin(a) * 4.4, S)
    cv.outline()
    return cv


# ───────────────────────────── Proyectiles ─────────────────────────────
def petal():
    cv = Canvas(16)
    cv.circle(6, 8, 4, O)
    for xx in range(6, 14):
        half = 4 * (1 - (xx - 6) / 8) ** 0.8
        for yy in range(16):
            if abs(yy + 0.5 - 8) <= half:
                cv.set(xx, yy, O)
    cv.ellipse(6, 9.5, 3.5, 1.6, o)
    cv.ellipse(7, 6.5, 2.5, 1.2, Y)
    cv.pixels([(xx, 8) for xx in range(4, 12)], o)
    cv.outline()
    return cv


def bomb():
    cv = Canvas(16)
    cv.circle(7.5, 9, 5.5, D)
    cv.ellipse(4.5, 6.5, 1.5, 1.2, L)
    cv.ellipse(7.5, 9, 3, 2.6, W)
    cv.rect(6, 11, 9, 12, W)
    cv.pixels([(6, 9), (9, 9)], K)
    cv.pixels([(7, 12), (8, 12)], K)
    cv.pixels([(5, 10), (10, 10)], R)
    cv.rect(10, 2, 11, 4, d)
    cv.pixels([(12, 1), (13, 0)], n)
    cv.outline()
    cv.pixels([(14, 0), (13, 1)], Y)
    return cv


def bone():
    cv = Canvas(16)
    cv.rect(4, 7, 11, 8, W)
    for bx in (3.5, 12.5):
        cv.circle(bx, 6.5, 1.7, W)
        cv.circle(bx, 9.5, 1.7, W)
    cv.rect(4, 8, 11, 8, w)
    cv.outline()
    return cv


def spark():
    cv = Canvas(16)
    cv.tri((8, 1), (6, 8), (10, 8), Y)
    cv.tri((8, 15), (6, 8), (10, 8), Y)
    cv.tri((1, 8), (8, 6), (8, 10), R)
    cv.tri((15, 8), (8, 6), (8, 10), T)
    cv.circle(8, 8, 2.2, F)
    cv.outline()
    return cv


# ───────────────────────────── Efectos ─────────────────────────────
def explosion(f):
    cv = Canvas(32)
    radius = [5, 8, 11, 13, 14][f]
    for yy in range(32):
        for xx in range(32):
            dist = math.hypot(xx + 0.5 - 16, yy + 0.5 - 16)
            if dist > radius:
                continue
            ring = dist / radius
            noise = h2(xx, yy, f)
            if f >= 3 and noise < 0.25 + (f - 3) * 0.3:
                continue                                   # se deshace al final
            if ring > 0.75:
                col = o if f < 3 else (90, 80, 96)
            elif ring > 0.45:
                col = O if f < 4 else (120, 110, 126)
            else:
                col = F if f < 2 else Y
            cv.set(xx, yy, col)
    return cv


def pulse(f):
    """Anillo de petalos que se expande (64 px = 1 unidad: se escala al rango de la torre)."""
    cv = Canvas(64)
    radius = [12, 20, 26, 30][f]
    count = 18
    for i in range(count):
        a = i * math.tau / count + f * 0.15
        px_ = 32 + math.cos(a) * radius
        py_ = 32 + math.sin(a) * radius
        cv.circle(px_, py_, 1.8, O if i % 2 else Y)
    alpha = [220, 200, 150, 90][f]
    for yy in range(64):
        for xx in range(64):
            col = cv.px[yy][xx]
            if col is not None:
                cv.px[yy][xx] = col + (alpha,)
    return cv


def ring(fill_alpha, edge_alpha, dither=False):
    """Circulo blanco semitransparente (el color lo pone el juego). 64 px = 1 unidad."""
    cv = Canvas(64)
    for yy in range(64):
        for xx in range(64):
            dist = math.hypot(xx + 0.5 - 32, yy + 0.5 - 32)
            if dist > 31.5:
                continue
            if dist > 29.5:
                cv.px[yy][xx] = (255, 255, 255, edge_alpha)
            elif not dither or (xx + yy) % 2 == 0:
                cv.px[yy][xx] = (255, 255, 255, fill_alpha)
    return cv


# ───────────────────────────── Mapa ─────────────────────────────
def tile(base, dark, light, seed, extras=None):
    cv = Canvas(32)
    for yy in range(32):
        for xx in range(32):
            v = h2(xx, yy, seed)
            cv.px[yy][xx] = dark if v < 0.18 else light if v > 0.88 else base
    if extras:
        extras(cv)
    return cv


def grass():
    def blades(cv):
        for i in range(14):
            bx = int(h2(i, 1, 9) * 32)
            by = int(h2(i, 2, 9) * 32)
            cv.set(bx, by, (72, 112, 70))
            cv.set(bx, (by - 1) % 32, (88, 132, 82))
        for i in range(2):
            fx = int(h2(i, 3, 9) * 30) + 1
            fy = int(h2(i, 4, 9) * 30) + 1
            cv.set(fx, fy, O)
    return tile((44, 70, 52), (34, 56, 42), (58, 88, 62), 3, blades)


def dirt_path():
    def petals(cv):
        for i in range(6):
            px_ = int(h2(i, 5, 4) * 32)
            py_ = int(h2(i, 6, 4) * 32)
            cv.set(px_, py_, O if i % 2 else Y)
    return tile((120, 86, 56), (98, 68, 44), (140, 104, 70), 5, petals)


def earth():
    def cracks(cv):
        for i in range(3):
            x0 = int(h2(i, 7, 2) * 28)
            y0 = int(h2(i, 8, 2) * 28)
            cv.line(x0, y0, x0 + 3, y0 + 2, (120, 74, 48))
    return tile((160, 104, 70), (138, 88, 58), (178, 122, 84), 8, cracks)


def cobble():
    cv = Canvas(32)
    mortar = (64, 58, 72)
    for yy in range(32):
        for xx in range(32):
            cv.px[yy][xx] = mortar
    for row in range(4):
        shift = 4 if row % 2 else 0
        for col in range(4):
            cx = (col * 8 + shift + 4) % 32
            cy = row * 8 + 4
            shade = 0.85 + h2(row, col, 11) * 0.3
            stone = tuple(min(255, int(v * shade)) for v in (128, 124, 140))
            for dy in range(-3, 4):
                for dx in range(-4, 4):
                    if abs(dx + 0.5) / 3.9 + abs(dy) / 3.6 < 1.35:
                        cv.set((cx + dx) % 32, cy + dy, stone)
            cv.set((cx - 2) % 32, cy - 2, (160, 156, 172))
    return cv


def tombstone():
    cv = Canvas(32)
    stone, stone2 = (122, 122, 142), (92, 92, 112)
    cv.ellipse(16, 11, 8, 7, stone)
    cv.rect(8, 11, 23, 26, stone)
    cv.rect(19, 9, 23, 26, stone2, only=[stone])
    cv.rect(15, 9, 16, 20, stone2)
    cv.rect(12, 12, 19, 13, stone2)
    cv.rect(6, 26, 25, 28, (60, 96, 60))
    cv.pixels([(7, 25), (10, 25), (22, 25), (24, 25)], (80, 124, 76))
    flower(cv, 9, 25, O, Y)
    cv.outline()
    return cv


def wood_cross():
    cv = Canvas(32)
    cv.rect(14, 4, 17, 28, N)
    cv.rect(8, 10, 23, 13, N)
    cv.rect(16, 4, 17, 28, n)
    for i in range(10):                                # corona de flores
        a = i * math.tau / 10
        flower(cv, 15.5 + math.cos(a) * 4.5, 11.5 + math.sin(a) * 4.5, O if i % 2 else Y, R)
    cv.rect(10, 28, 21, 29, (60, 96, 60))
    cv.outline()
    return cv


def candle():
    cv = Canvas(16)
    cv.rect(6, 6, 9, 14, W)
    cv.rect(8, 6, 9, 14, w)
    cv.ellipse(7.5, 4, 1.6, 2.4, Y)
    cv.ellipse(7.5, 4.5, 0.8, 1.2, F)
    cv.rect(4, 14, 11, 15, C)
    cv.outline()
    return cv


def marigold_bush():
    cv = Canvas(32)
    cv.ellipse(16, 22, 11, 6, G)
    cv.ellipse(16, 24, 9, 3.5, g)
    for i, (fx, fy) in enumerate([(9, 19), (14, 17), (20, 18), (24, 21), (11, 23), (18, 22), (16, 13)]):
        cv.circle(fx, fy, 2.8, O if i % 2 else o)
        cv.circle(fx, fy, 1.2, Y)
    cv.outline()
    return cv


def flower_pot():
    cv = Canvas(32)
    cv.tri((8, 18), (24, 18), (16, 31), C)
    cv.rect(10, 18, 22, 28, C)
    cv.rect(8, 17, 24, 19, c)
    cv.pixels([(12, 23), (16, 23), (20, 23)], Y)
    for i, (fx, fy) in enumerate([(11, 11), (16, 8), (21, 11), (13, 15), (19, 15)]):
        cv.line(fx, fy, 16, 18, G)
        cv.circle(fx, fy, 2.8, O if i % 2 else o)
        cv.circle(fx, fy, 1.1, Y)
    cv.outline()
    return cv


def sugar_skull():
    cv = Canvas(16)
    cv.ellipse(8, 7, 5, 5, W)
    cv.rect(5, 9, 10, 12, W)
    cv.circle(6, 7, 1.6, B)
    cv.circle(10, 7, 1.6, R)
    cv.pixels([(6, 7), (10, 7)], K)
    cv.pixels([(8, 9)], K)
    cv.pixels([(6, 11), (8, 11), (10, 11)], K)
    cv.pixels([(8, 3), (7, 4), (9, 4)], O)
    cv.outline()
    return cv


# ───────────────────────────── UI (9-slice, borde de 5 px) ─────────────────────────────
def ui_panel():
    cv = Canvas(16)
    cv.rect(0, 0, 15, 15, K)
    cv.rect(1, 1, 14, 14, o)
    cv.rect(2, 2, 13, 13, (54, 30, 70))
    cv.rect(3, 3, 12, 12, (42, 24, 56))
    for xx, yy in ((0, 0), (15, 0), (0, 15), (15, 15)):
        cv.set(xx, yy, None)
    cv.pixels([(2, 2), (13, 2), (2, 13), (13, 13)], O)
    return cv


def ui_button():
    cv = Canvas(16)
    cv.rect(0, 0, 15, 15, K)
    cv.rect(1, 1, 14, 14, (244, 228, 193))
    cv.rect(1, 1, 14, 1, (255, 248, 226))
    cv.rect(1, 12, 14, 14, (206, 176, 134))
    for xx, yy in ((0, 0), (15, 0), (0, 15), (15, 15)):
        cv.set(xx, yy, None)
    return cv


# ───────────────────────────── Lista ─────────────────────────────
def frames(fn, count, *args):
    return [lambda i=i: fn(i, *args) for i in range(count)]


SPRITES = {}
for name, fn in [("Catrina", catrina), ("CalaveritaCanonera", calaverita), ("Cempasuchil", cempasuchil),
                 ("Xolo", xolo), ("Alebrije", alebrije), ("Ofrenda", ofrenda)]:
    for i, make in enumerate(frames(fn, 2)):
        SPRITES[f"Towers/{name}_{i}"] = make
for kind, name in [("white", "GhostWhite"), ("blue", "GhostBlue"), ("green", "GhostGreen"), ("purple", "GhostPurple")]:
    for i in range(2):
        SPRITES[f"Enemies/{name}_{i}"] = lambda i=i, kind=kind: ghost(kind, i)
for i in range(2):
    SPRITES[f"Enemies/Pumpkin_{i}"] = lambda i=i: pumpkin(i)
    SPRITES[f"Enemies/Pumpkinling_{i}"] = lambda i=i: pumpkin(i, small=True)
    SPRITES[f"Enemies/Skeleton_{i}"] = lambda i=i: skeleton(i)
    SPRITES[f"Enemies/SkeletonWarrior_{i}"] = lambda i=i: skeleton(i, warrior=True)
    SPRITES[f"Enemies/Banshee_{i}"] = lambda i=i: banshee(i)
    SPRITES[f"Enemies/CharroNegro_{i}"] = lambda i=i: charro(i)
SPRITES.update({
    "Projectiles/Petal": petal, "Projectiles/Bomb": bomb, "Projectiles/Bone": bone, "Projectiles/Spark": spark,
    "Indicators/Range": lambda: ring(150, 255), "Indicators/Aura": lambda: ring(255, 255, dither=True),
    "Tiles/Grass": grass, "Tiles/DirtPath": dirt_path, "Tiles/Earth": earth, "Tiles/Cobble": cobble,
    "Decor/Tombstone": tombstone, "Decor/Cross": wood_cross, "Decor/Candle": candle,
    "Decor/MarigoldBush": marigold_bush, "Decor/FlowerPot": flower_pot, "Decor/SugarSkull": sugar_skull,
    "UI/Panel": ui_panel, "UI/Button": ui_button,
})
for i in range(5):
    SPRITES[f"Effects/Explosion_{i}"] = lambda i=i: explosion(i)
for i in range(4):
    SPRITES[f"Indicators/Pulse_{i}"] = lambda i=i: pulse(i)

if __name__ == "__main__":
    out = sys.argv[1]
    for name, make in SPRITES.items():
        make().save(os.path.join(out, name + ".png"))
    print(f"{len(SPRITES)} sprites en {out}")
