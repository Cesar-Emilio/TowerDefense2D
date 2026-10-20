-- Une cuadros PNG en un solo archivo .aseprite animado.
-- Uso: Aseprite -b --script-param files="a_0.png;a_1.png" --script-param out="a.aseprite" --script-param fps=6 --script build_aseprite.lua
local files = {}
for f in string.gmatch(app.params["files"], "[^;]+") do table.insert(files, f) end
local fps = tonumber(app.params["fps"] or "6")

local first = Image{ fromFile = files[1] }
local sprite = Sprite(first.width, first.height, ColorMode.RGB)
local layer = sprite.layers[1]

for i, file in ipairs(files) do
  if i > 1 then sprite:newEmptyFrame() end
  sprite:newCel(layer, i, Image{ fromFile = file }, Point(0, 0))
  sprite.frames[i].duration = 1 / fps
end

sprite:saveAs(app.params["out"])
sprite:close()
